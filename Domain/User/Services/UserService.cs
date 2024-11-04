using DotNetService.Http.API.Version1.User;
using DotNetService.Domain.User.Repositories;
using DotNetService.Http.API.Version1;
using DotNetService.Infrastructure.Shareds;
using System.Net;
using DotNetService.Infrastructure.Exceptions;

namespace DotNetService.Domain.User.Services
{
    public class UserService(
        UserQueryRepository userQueryRepository,
        UserStoreRepository userStoreRepository
    )
    {
        private readonly UserQueryRepository _userQueryRepository = userQueryRepository;
        private readonly UserStoreRepository _userStoreRepository = userStoreRepository;

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

        public async Task Create(UserCreateRequest dataCreate)
        {
            var isEmailExist = await _userQueryRepository.IsEmailExists(dataCreate.Email);

            if (isEmailExist)
            {
                throw new UnprocessableEntityException("Email already exist");
            }

            var data = UserCreateRequest.Assign(dataCreate);
            await _userStoreRepository.Create(data, dataCreate.RoleIds);
        }

        public async Task<Models.User> Detail(Guid id)
        {
            return await _userQueryRepository.FindOneById(id, true);
        }

        public async Task Update(Guid id, UserUpdateRequest dataUpdate)
        {
            var isEmailExist = await _userQueryRepository.IsEmailExistsExceptId(dataUpdate.Email, id);

            if (isEmailExist)
            {
                throw new UnprocessableEntityException("Email already exist");
            }

            var data = UserUpdateRequest.Assign(dataUpdate);
            await _userStoreRepository.Update(id, data, dataUpdate.RoleIds);
        }

        public async Task Delete(Guid id)
        {
            await _userStoreRepository.Delete(id);
        }
    }
}