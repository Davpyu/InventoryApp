using System.Net;
using DotNetService.Domain.Role.Repositories;
using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1.Role;
using DotNetService.Http.API.Version1.Role.Requests;
using DotNetService.Infrastructure.Exceptions;
using DotNetService.Infrastructure.Shareds;

namespace DotNetService.Domain.Role.Services
{
    public class RoleService(
        RoleStoreRepository roleStoreRepository,
        RoleQueryRepository roleQueryRepository
    )
    {
        private readonly RoleStoreRepository _roleStoreRepository = roleStoreRepository;
        private readonly RoleQueryRepository _roleQueryRepository = roleQueryRepository;

        public async Task<ApiResponse> Index(RoleQueryRequest query = null)
        {
            var data = await Pagination(query);
            int count = await _roleQueryRepository.Count(query);
            decimal pageInCount = ((decimal)count) / query.PerPage;
            PaginationModel paginate = new()
            {
                TotalPage = (int)Math.Ceiling(pageInCount),
                Page = query.Page,
                PerPage = query.PerPage,
                Data = RoleResponse.MapRepo(data),
                Total = count
            };

            return new ApiResponsePagination(HttpStatusCode.OK, paginate);
        }

        public async Task<List<Models.Role>> Pagination(RoleQueryRequest query = null)
        {
            return await _roleQueryRepository.Pagination(query);
        }

        public async Task<Models.Role> Create(RoleCreateRequest dataCreate)
        {
            var isRoleExist = await _roleQueryRepository.IsExistByKey(dataCreate.Key);

            if (isRoleExist)
            {
                throw new UnprocessableEntityException("Role key already exist");
            }

            var data = RoleCreateRequest.Assign(dataCreate);

            var roleCreated = await _roleStoreRepository.Create(data, dataCreate.PermissionIds);

            return await DetailById(roleCreated.Id);
        }

        public async Task<Models.Role> DetailById(Guid id)
        {
            return await _roleQueryRepository.FindOneById(id);
        }

        public async Task<List<Models.Role>> GetList(string search, int page, int perPage)
        {
            return await _roleQueryRepository.Get(search, page, perPage);
        }
        public async Task<int> Count(string search)
        {
            return await _roleQueryRepository.CountAll(search);
        }

        public async Task Update(Guid id, RoleUpdateRequest dataUpdate)
        {
            var data = RoleUpdateRequest.Assign(dataUpdate);
            await _roleStoreRepository.Update(id, data, dataUpdate.PermissionIds);
        }

        public async Task Delete(Guid id)
        {
            await _roleStoreRepository.Delete(id);
        }
    }
}