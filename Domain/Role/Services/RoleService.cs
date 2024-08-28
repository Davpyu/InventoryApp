using System.Net;
using DotNetService.Domain.Permission.Repositories;
using DotNetService.Domain.Role.Repositories;
using DotNetService.Domain.RolePermission.Repositories;
using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1.Role;
using DotNetService.Infrastructure.Shareds;

namespace DotNetService.Domain.Role.Services
{
    public class RoleService(
        RoleStoreRepository roleStoreRepository,
        RoleQueryRepository roleQueryRepository,
        RolePermissionStoreRepository rolePermissionStoreRepository,
        RolePermissionQueryRepository rolePermissionQueryRepository
    )
    {
        private readonly RoleStoreRepository _roleStoreRepository = roleStoreRepository;
        private readonly RoleQueryRepository _roleQueryRepository = roleQueryRepository;
        private readonly RolePermissionStoreRepository _rolePermissionStoreRepository = rolePermissionStoreRepository;
        private readonly RolePermissionQueryRepository _rolePermissionQueryRepository = rolePermissionQueryRepository;

        public ApiResponse Index(RoleQueryRequest query = null)
        {
            var data = this.Pagination(query);
            int count = _roleQueryRepository.Count(query);
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

        public List<Models.Role> Pagination(RoleQueryRequest query = null)
        {
            return _roleQueryRepository.Pagination(query);
        }

        public Models.Role Create(RoleCreateRequest dataCreate)
        {
            var data = RoleCreateRequest.Assign(dataCreate);

            var roleCreated = _roleStoreRepository.Create(data);
            if (dataCreate.PermissionIds?.Count > 0)
            {
                var rolePermissions = new List<Models.RolePermission>();
                foreach (var permissionId in dataCreate.PermissionIds)
                {
                    var rolePermission = new Models.RolePermission
                    {
                        Roleid = roleCreated.Id,
                        Permissionid = permissionId
                    };
                    rolePermissions.Add(rolePermission);
                }
                _rolePermissionStoreRepository.BulkSave(rolePermissions.ToArray());
            }

            return this.DetailById(roleCreated.Id);
            
        }

        public Models.Role DetailById(Guid id)
        {
            return _roleQueryRepository.FindOneById(id);
        }

        public List<Models.Role> GetList(string search, int page, int perPage)
        {
            return _roleQueryRepository.Get(search, page, perPage);
        }
        public int Count(string search)
        {
            return _roleQueryRepository.CountAll(search);
        }

        public Models.Role Update(Guid id, RoleUpdateRequest dataUpdate)
        {
            var data = RoleUpdateRequest.Assign(dataUpdate);
            _roleStoreRepository.Update(id, data);
            var role = this.DetailById(id);
            if(role.RolePermissions?.Count > 0){
                var rolePermissionsToDelete = _rolePermissionQueryRepository.FindByRoleId(id);
                _rolePermissionStoreRepository.DeleteBulk(rolePermissionsToDelete);
            }
            if (dataUpdate.PermissionIds?.Count > 0)
            {
                var newRolePermissions = new List<Models.RolePermission>();
                foreach (var permissionId in dataUpdate.PermissionIds)
                {
                    var rolePermission = new Models.RolePermission
                    {
                        Roleid = id,
                        Permissionid = permissionId
                    };

                    newRolePermissions.Add(rolePermission);
                }
                _rolePermissionStoreRepository.BulkSave(newRolePermissions.ToArray());
            }
            return this.DetailById(id);
        }

        public void Delete(Guid id)
        {
            _roleStoreRepository.Delete(id);
        }
    }
}