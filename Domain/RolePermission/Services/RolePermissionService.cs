using DotNetService.Http.API.Version1.RolePermission;
using DotNetService.Domain.RolePermission.Repositories;

namespace DotNetService.Applications.RolePermission.Service
{
    public class RolePermissionService(
        RolePermissionStoreRepository rolePermissionStoreRepository,
        RolePermissionQueryRepository rolePermissionQueryRepository
    )
    {
        private readonly RolePermissionStoreRepository _rolePermissionStoreRepository =
            rolePermissionStoreRepository;
        private readonly RolePermissionQueryRepository _rolePermissionQueryRepository =
            rolePermissionQueryRepository;

        public async Task Create(RolePermissionCreateRequest rolePermissionCreate)
        {
            var rolePermissionRepository = new Models.RolePermission
            {
                Roleid = rolePermissionCreate.Roleid,
                Permissionid = rolePermissionCreate.Permissionid
            };

            await _rolePermissionStoreRepository.Create(rolePermissionRepository);
        }

        public async Task<Models.RolePermission> DetailById(Guid id)
        {
            return await _rolePermissionQueryRepository.FindById(id);
        }

        public async Task<Models.RolePermission> DetailByRolePermission(Guid roleid, Guid permissionid)
        {
            return await _rolePermissionQueryRepository.FindByRoleAndPermission(roleid, permissionid);
        }

        public async Task<List<Models.RolePermission>> GetList(int page, int perPage)
        {
            return await _rolePermissionQueryRepository.Get(page, perPage);
        }

        public async Task<int> Count()
        {
            return await _rolePermissionQueryRepository.CountAll();
        }

        public async Task Update(Guid id, RolePermissionUpdateRequest rolePermissionUpdate)
        {
            Models.RolePermission rolePermissionRepository = new Models.RolePermission
            {
                Roleid = rolePermissionUpdate.Roleid,
                Permissionid = rolePermissionUpdate.Permissionid
            };

           await _rolePermissionStoreRepository.Update(id, rolePermissionRepository);
        }

        public async Task Delete(Guid id)
        {
           await _rolePermissionStoreRepository.Delete(id);
        }
    }
}