using System.Data.Entity.Infrastructure;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetService.Domain.Permission.Repositories
{
    public class PermissionStoreRepository(
        PermissionQueryRepository permissionQueryRepository,
        Models.IamDBContext context
    )
    {
        private readonly PermissionQueryRepository _permissionQueryRepository = permissionQueryRepository;
        private readonly Models.IamDBContext _context = context;

        public async Task<Models.Permission> Create(Models.Permission permissionRepository)
        {
            Models.Permission newPermission = new()
            {
                Name = permissionRepository.Name
            };

            return await this.Save(newPermission);
        }

        public async Task Update(Guid id, Models.Permission permissionRepository)
        {
            Models.Permission oldPermission = await _permissionQueryRepository.Find(id);
            if (oldPermission == null)
            {
                return;
            }

            oldPermission.Name = permissionRepository.Name;
            await this.Save(oldPermission, true);
        }

        public async Task Delete(Guid id)
        {
            try
            {
                Models.Permission data = new Models.Permission { Id = id };
                _context.Permissions.Attach(data);
                _context.Permissions.Remove(data);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }

        private async Task<Models.Permission> Save(Models.Permission data, bool isUpdate = false)
        {
            if (!isUpdate)
            {

                var dataCreated = _context.Permissions.Add(data);
                await _context.SaveChangesAsync();
                return dataCreated.Entity;
            }

            try
            {
                var dataUpdated = _context.Permissions.Update(data);
                await _context.SaveChangesAsync();

                return dataUpdated.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException(
                    "No data was updated."
                );
            }
        }

        public async Task BulkSave(Models.Permission[] data)
        {
            _context.Permissions.AddRange(data);
            await _context.SaveChangesAsync();
        }
    }
}