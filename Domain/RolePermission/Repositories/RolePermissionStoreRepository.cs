using System.Data.Entity.Infrastructure;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

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
            try
            {
                Models.RolePermission data = new Models.RolePermission { Id = id };
                _context.RolePermissions.Attach(data);
                _context.RolePermissions.Remove(data);
                _context.SaveChanges();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
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