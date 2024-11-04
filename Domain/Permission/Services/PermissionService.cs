using DotNetService.Http.API.Version1.Permission;
using DotNetService.Domain.Permission.Repositories;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Http.API.Version1;
using System.Net;
using DotNetService.Infrastructure.Exceptions;

namespace DotNetService.Domain.Permission.Services
{
    public class PermissionService(
        PermissionStoreRepository permissionStoreRepository,
        PermissionQueryRepository permissionQueryRepository
    )
    {
        private readonly PermissionStoreRepository _permissionStoreRepository = permissionStoreRepository;
        private readonly PermissionQueryRepository _permissionQueryRepository = permissionQueryRepository;

        public async Task<ApiResponse> Index(PermissionQueryRequest query = null)
        {
            var data = await Pagination(query);
            int count = await _permissionQueryRepository.Count(query);
            decimal pageInCount = ((decimal)count) / query.PerPage;
            PaginationModel paginate = new()
            {
                TotalPage = (int)Math.Ceiling(pageInCount),
                Page = query.Page,
                PerPage = query.PerPage,
                Data = PermissionResponse.MapRepo(data),
                Total = count
            };

            return new ApiResponsePagination(HttpStatusCode.OK, paginate);
        }

        public async Task<List<Models.Permission>> Pagination(PermissionQueryRequest query = null)
        {
            return await _permissionQueryRepository.Pagination(query);
        }

        public async Task Create(PermissionCreateRequest dataCreate)
        {
            var data = PermissionCreateRequest.Assign(dataCreate);

            await _permissionStoreRepository.Create(data);
        }

        public async Task<Models.Permission> DetailById(Guid id)
        {
            return await _permissionQueryRepository.FindOneById(id) ?? throw new DataNotFoundException("Permission not found");
        }

        public async Task<List<Models.Permission>> GetList(string search, int page, int perPage)
        {
            return await _permissionQueryRepository.Get(search, page, perPage);
        }
        public async Task<int> Count(string search)
        {
            return await _permissionQueryRepository.CountAll(search);
        }

        public async Task Update(Guid id, PermissionUpdateRequest dataUpdate)
        {
            var data = PermissionUpdateRequest.Assign(dataUpdate);
            await _permissionStoreRepository.Update(id, data);
        }

        public async Task Delete(Guid id)
        {
            await _permissionStoreRepository.Delete(id);
        }
    }
}