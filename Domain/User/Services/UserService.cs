using DotNetService.Http.API.Version1.User;
using DotNetService.Domain.User.Repositories;
using DotNetService.Http.API.Version1;
using DotNetService.Infrastructure.Shareds;
using System.Net;
using BC = BCrypt.Net.BCrypt;
using DotNetService.Domain.UserRole.Repositories;

namespace DotNetService.Domain.User.Services
{
    public class UserService(
        UserQueryRepository userQueryRepository,
        UserStoreRepository userStoreRepository,
        UserRoleStoreRepository userRoleStoreRepository,
        UserRoleQueryRepository userRoleQueryRepository
    )
    {
        private readonly UserQueryRepository _userQueryRepository = userQueryRepository;
        private readonly UserStoreRepository _userStoreRepository = userStoreRepository;
        private readonly UserRoleStoreRepository _userRoleStoreRepository = userRoleStoreRepository;
        private readonly UserRoleQueryRepository _userRoleQueryRepository = userRoleQueryRepository;

        public async Task<ApiResponse> Index(UserQueryRequest query = null)
        {
            var data = await _userQueryRepository.Pagination(query);
            int count = await _userQueryRepository.Count(query);
            decimal pageInCount = ((decimal)count) / query.PerPage;
            PaginationModel paginate = new()
            {
                TotalPage = (int)Math.Ceiling(pageInCount),
                Page = query.Page,
                PerPage = query.PerPage,
                Data = UserResponse.MapRepo(data),
                Total = count
            };

            return new ApiResponsePagination(HttpStatusCode.OK, paginate);
        }

        public async Task<Models.User> Create(UserCreateRequest dataCreate)
        {
            var data = UserCreateRequest.Assign(dataCreate);
            var user = await _userStoreRepository.Create(data);
            if (dataCreate.RoleIds?.Count > 0)
            {
                var userRoles = new List<Models.UserRole>();
                foreach (var roleId in dataCreate.RoleIds)
                {
                    var userRole = new Models.UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    };

                    userRoles.Add(userRole);
                }
                await _userRoleStoreRepository.BulkSave(userRoles.ToArray());
            }
            return await Detail(user.Id);
        }

        public async Task<Models.User> Detail(Guid id)
        {
            return await _userQueryRepository.FindOneById(id, true);
        }

        public async Task<Models.User> Update(Guid id, UserUpdateRequest dataUpdate)
        {
            var data = UserUpdateRequest.Assign(dataUpdate);
            var updatedData = await _userStoreRepository.Update(id, data);
            var user = await Detail(id);
            var userRoles = user?.UserRoles;
            if (userRoles?.Count > 0)
            {
                var userRolesToDelete = await _userRoleQueryRepository.FindByUserId(id);
                await _userRoleStoreRepository.DeleteBulk(userRolesToDelete);
            }
            if (dataUpdate.RoleIds?.Count > 0)
            {
                var newUserRoles = new List<Models.UserRole>();
                foreach (var roleId in dataUpdate.RoleIds)
                {
                    var userRole = new Models.UserRole
                    {
                        UserId = updatedData.Id,
                        RoleId = roleId
                    };

                    newUserRoles.Add(userRole);
                }
                await _userRoleStoreRepository.BulkSave(newUserRoles.ToArray());
            }
            return await Detail(updatedData.Id);

        }

        public async Task Delete(Guid id)
        {
            await _userStoreRepository.Delete(id);
        }
    }
}