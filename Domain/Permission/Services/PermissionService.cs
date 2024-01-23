using DotNetService.Http.API.Version1.Requests.Permission;
using DotNetService.Domain.Permission.Repositories;

namespace DotNetService.Domain.Permission.Services
{
    public class PermissionService
    {
        private readonly PermissionStoreRepository _permissionStoreRepository;
        private readonly PermissionQueryRepository _permissionQueryRepository;
        
        public void Create(PermissionCreate permissionCreate)
        {
            var permissionRepository = new Models.Permission
            {
                Name = permissionCreate.Name
            };
            
            _permissionStoreRepository.Create(permissionRepository);
        }

        public Models.Permission DetailById(Guid id)
        {
            return _permissionQueryRepository.FindById(id);
        }

        public List<Models.Permission> GetList(string search, int page, int perPage)
        {
            return _permissionQueryRepository.Get(search, page, perPage);
        }
        public int Count(string search)
        {
            return _permissionQueryRepository.CountAll(search);
        }

        public void Update(Guid id, PermissionUpdate permissionUpdate)
        {
            Models.Permission permissionRepository = new Models.Permission();
            permissionRepository.Name = permissionUpdate.Name;
            _permissionStoreRepository.Update(id, permissionRepository);
        }

        public void Delete(Guid id)
        {
            _permissionStoreRepository.Delete(id);
        }
    }
}