using Models = DotNetService.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DotNetService.Domain.RolePermission.Repositories
{
    public class RolePermissionStoreRepository
    {
        private readonly Models.IamDBContext _context;
        private readonly RolePermissionQueryRepository _rolePermissionQueryRepository;

        public RolePermissionStoreRepository(
            Models.IamDBContext context,
            RolePermissionQueryRepository rolePermissionQueryRepository
        )
        {
            _context = context;
            _rolePermissionQueryRepository = rolePermissionQueryRepository;
        }

        public void Create(Models.RolePermission rolePermission)
        {
            this.Save(rolePermission);
        }

        public void Update(Guid id, Models.RolePermission rolePermission)
        {
            Models.RolePermission oldRolePermission = _rolePermissionQueryRepository.Find(id);
            if (oldRolePermission == null)
            {
                return;
            }

            this.Save(rolePermission, true);
        }

        public void Delete(Guid id)
        {
            Models.RolePermission rolePermission = _context.RolePermissions.Where(rolePermission => rolePermission.Id == id).FirstOrDefault();
            _context.RolePermissions.Remove(rolePermission);
            _context.SaveChanges();
        }

        private void Save(Models.RolePermission RolePermission, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.RolePermissions.Add(RolePermission);
            }
            _context.SaveChanges();
        }
    }
}