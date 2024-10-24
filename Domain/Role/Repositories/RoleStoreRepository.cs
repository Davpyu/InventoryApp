using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetService.Domain.Role.Repositories
{
    public class RoleStoreRepository
    {
        private readonly Models.IamDBContext _context;

        public RoleStoreRepository(
            Models.IamDBContext context,
            RoleQueryRepository roleQueryRepository
        )
        {
            _context = context;
        }

        public async Task<Models.Role> Create(Models.Role role, List<Guid> permissionIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newRole = await _context.Roles.AddAsync(new Models.Role
                {
                    Id = Guid.NewGuid(),
                    Name = role.Name,
                    Key = role.Key
                });
                var createdRole = newRole.Entity;

                if (permissionIds?.Count > 0)
                {
                    var rolePermissions = new List<Models.RolePermission>();
                    foreach (var permissionId in permissionIds)
                    {
                        var rolePermission = new Models.RolePermission
                        {
                            RoleId = createdRole.Id,
                            PermissionId = permissionId
                        };
                        rolePermissions.Add(rolePermission);
                    }
                    await _context.RolePermissions.AddRangeAsync(rolePermissions);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return createdRole;
            }
            catch (Exception)
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task Update(Guid id, Models.Role roleRepository)
        {
            try
            {
                Models.Role data = new Models.Role { Id = id };
                _context.Roles.Attach(data);
                _context.Roles.Update(roleRepository);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was updated.");
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                Models.Role data = new Models.Role { Id = id };
                _context.Roles.Attach(data);
                _context.Roles.Remove(data);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }
    }
}