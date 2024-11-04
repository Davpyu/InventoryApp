using System.Data.Entity.Infrastructure;
using DotNetService.Infrastructure.Exceptions;
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

        public async Task Create(Models.RolePermission rolePermission)
        {
            await Save(rolePermission);
        }

        public async Task Update(Guid id, Models.RolePermission rolePermission)
        {
            Models.RolePermission oldRolePermission = await _rolePermissionQueryRepository.Find(id);
            if (oldRolePermission == null)
            {
                return;
            }

            await Save(rolePermission, true);
        }

        public async Task Delete(Guid id)
        {
            try
            {
                Models.RolePermission data = new Models.RolePermission { Id = id };
                _context.RolePermissions.Attach(data);
                _context.RolePermissions.Remove(data);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }

        public async Task DeleteBulk(List<Models.RolePermission> data)
        {
            _context.RolePermissions.RemoveRange(data);
            await _context.SaveChangesAsync();
        }

        private async Task Save(Models.RolePermission data, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.RolePermissions.Add(data);
                await _context.SaveChangesAsync();
            }
            try
            {
                var dataUpdated = _context.RolePermissions.Update(data);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException(
                    "No data was updated."
                );
            }
        }

        public async Task BulkSave(Models.RolePermission[] data)
        {
            _context.RolePermissions.AddRange(data);
            await _context.SaveChangesAsync();
        }
    }
}