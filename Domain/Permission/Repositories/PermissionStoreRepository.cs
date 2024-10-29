using System.Data.Entity;
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

        public async Task Create(Models.Permission permissionRepository)
        {
            Models.Permission newPermission = new()
            {
                Name = permissionRepository.Name,
                Key = permissionRepository.Key
            };

            await _context.Permissions.AddAsync(newPermission);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Guid id, Models.Permission permissionRepository)
        {
            try
            {
                Models.Permission data = new() { Id = id };
                _context.Permissions.Attach(data);
                _context.Permissions.Update(permissionRepository);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was updated.");
            }
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

        public async Task BulkSave(Models.Permission[] data)
        {
            _context.Permissions.AddRange(data);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsExistByKey(string key)
        {
            return await _context.Permissions.AnyAsync(x => x.Key == key);
        }
    }
}