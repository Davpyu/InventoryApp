using DotNetService.Http.API.Version1.Permission;
using DotNetService.Domain.Permission.Repositories;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Http.API.Version1;
using System.Net;

namespace DotNetService.Domain.Permission.Services
{
    public class PermissionService(
        PermissionStoreRepository permissionStoreRepository,
        PermissionQueryRepository permissionQueryRepository
        )
    {
        private readonly PermissionStoreRepository _permissionStoreRepository = permissionStoreRepository;
        private readonly PermissionQueryRepository _permissionQueryRepository = permissionQueryRepository;

        public ApiResponse Index(PermissionQueryRequest query = null)
        {
            if (query.Pagination)
            {
                var data = this.Pagination(query);
                int count = _permissionQueryRepository.Count(query);
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
            else
            {
                var data = Pagination(query);
                return new ApiResponseDataList(HttpStatusCode.OK, data, data.Count);
            }
        }

        public List<Models.Permission> Pagination(PermissionQueryRequest query = null)
        {
            return _permissionQueryRepository.Pagination(query);
        }
        
        public Models.Permission Create(PermissionCreateRequest dataCreate)
        {
            var data = PermissionCreateRequest.Assign(dataCreate);
            
            return _permissionStoreRepository.Create(data);
        }

        public Models.Permission DetailById(Guid id)
        {
            return _permissionQueryRepository.FindOneById(id);
        }

        public List<Models.Permission> GetList(string search, int page, int perPage)
        {
            return _permissionQueryRepository.Get(search, page, perPage);
        }
        public int Count(string search)
        {
            return _permissionQueryRepository.CountAll(search);
        }

        public void Update(Guid id, PermissionUpdateRequest dataUpdate)
        {
            var data = PermissionUpdateRequest.Assign(dataUpdate);
            _permissionStoreRepository.Update(id, data);
        }

        public void Delete(Guid id)
        {
            _permissionStoreRepository.Delete(id);
        }
    }
}