using System.Data.Entity.Infrastructure;
using DotNetService.Exceptions;

namespace DotNetService.Domain.RolePermission.Repositories
{
    public class RolePermissionStoreRepository(
        Models.IamDBContext context,
        RolePermissionQueryRepository rolePermissionQueryRepository
    )
    {
        private readonly Models.IamDBContext _context = context;
        private readonly RolePermissionQueryRepository _rolePermissionQueryRepository = rolePermissionQueryRepository;

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

        public void DeleteBulk(List<Models.RolePermission> data)
        {
            _context.RolePermissions.RemoveRange(data);
            _context.SaveChanges();
        }

        private void Save(Models.RolePermission data, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.RolePermissions.Add(data);
                _context.SaveChanges();
            }
            try
            {
                var dataUpdated = _context.RolePermissions.Update(data);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException(
                    "No data was updated."
                );
            }
        }

        public void BulkSave(Models.RolePermission[] data)
        {
            _context.RolePermissions.AddRange(data);
            _context.SaveChanges();
        }
    }
}