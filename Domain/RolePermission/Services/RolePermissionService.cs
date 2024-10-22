using DotNetService.Http.API.Version1.RolePermission;
using DotNetService.Domain.RolePermission.Repositories;

namespace DotNetService.Applications.RolePermission.Service
{
    public class RolePermissionService
    {
        private readonly RolePermissionStoreRepository _rolePermissionStoreRepository;
        private readonly RolePermissionQueryRepository _rolePermissionQueryRepository;

        public void Create(RolePermissionCreateRequest rolePermissionCreate)
        {
            var rolePermissionRepository = new Models.RolePermission
            {
                RoleId = rolePermissionCreate.RoleId,
                PermissionId = rolePermissionCreate.PermissionId
            };

            _rolePermissionStoreRepository.Create(rolePermissionRepository);
        }

        public Models.RolePermission DetailById(Guid id)
        {
            return _rolePermissionQueryRepository.FindById(id);
        }

        public Models.RolePermission DetailByRolePermission(Guid roleid, Guid permissionid)
        {
            return _rolePermissionQueryRepository.FindByRoleAndPermission(roleid, permissionid);
        }

        public List<Models.RolePermission> GetList(int page, int perPage)
        {
            return _rolePermissionQueryRepository.Get(page, perPage);
        }

        public int Count()
        {
            return _rolePermissionQueryRepository.CountAll();
        }

        public void Update(Guid id, RolePermissionUpdateRequest rolePermissionUpdate)
        {
            Models.RolePermission rolePermissionRepository = new Models.RolePermission
            {
                RoleId = rolePermissionUpdate.RoleId,
                PermissionId = rolePermissionUpdate.PermissionId
            };

            _rolePermissionStoreRepository.Update(id, rolePermissionRepository);
        }

        public void Delete(Guid id)
        {
            _rolePermissionStoreRepository.Delete(id);
        }
    }
}