using Models = DotNetService.Models;

namespace DotNetService.Domain.Permission.Repositories
{
    public class PermissionStoreRepository
    {
        private readonly PermissionQueryRepository _permissionQueryRepository;
        private readonly Models.MainDBContext _context;

        public PermissionStoreRepository(
            Models.MainDBContext context,
            PermissionQueryRepository permissionQueryRepository
        )
        {
            _context = context;
            _permissionQueryRepository = permissionQueryRepository;
        }

        public void Create(Models.Permission permissionRepository)
        {
            Models.Permission newPermission = new()
            {
                Name = permissionRepository.Name
            };
            
            this.Save(newPermission);
        }

        public void Update(Guid id, Models.Permission permissionRepository)
        {
            Models.Permission oldPermission = _permissionQueryRepository.Find(id);
            if (oldPermission == null)
            {
                return;
            }

            oldPermission.Name = permissionRepository.Name;
            this.Save(oldPermission, true);
        }

        public void Delete(Guid id)
        {
            Models.Permission permission = _context.Permissions.Where(permission => permission.Id == id).FirstOrDefault();
            _context.Permissions.Remove(permission);
            _context.SaveChanges();
        }

        private void Save(Models.Permission Permission, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.Permissions.Add(Permission);
            }
            _context.SaveChanges();
        }
    }
}