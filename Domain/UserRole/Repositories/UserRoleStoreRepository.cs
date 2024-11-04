using System.Data.Entity.Infrastructure;
using DotNetService.Infrastructure.Exceptions;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetService.Domain.UserRole.Repositories
{
    public class UserRoleStoreRepository(
        UserRoleQueryRepository userRoleQueryRepository,
        Models.IamDBContext context
    )
    {
        private readonly UserRoleQueryRepository _userRoleQueryRepository = userRoleQueryRepository;
        private readonly Models.IamDBContext _context = context;

        public async Task Create(Models.UserRole userRole)
        {
            await Save(userRole);
        }

        public async Task Update(Guid id, Models.UserRole userRole)
        {
            Models.UserRole oldUserRole = await _userRoleQueryRepository.Find(id);
            if (oldUserRole == null)
            {
                return;
            }

            await Save(userRole, true);
        }

        public async Task Delete(Guid id)
        {
            try
            {
                Models.UserRole data = new Models.UserRole { Id = id };
                _context.UserRoles.Attach(data);
                _context.UserRoles.Remove(data);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }

        public async Task DeleteBulk(List<Models.UserRole> data)
        {
            _context.UserRoles.RemoveRange(data);
            await _context.SaveChangesAsync();
        }

        private async Task Save(Models.UserRole data, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.UserRoles.Add(data);
                await _context.SaveChangesAsync();
            }
            try
            {
                var dataUpdated = _context.UserRoles.Update(data);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException(
                    "No data was updated."
                );
            }
        }

        public async Task BulkSave(Models.UserRole[] data)
        {
            _context.UserRoles.AddRange(data);
            await _context.SaveChangesAsync();
        }
    }
}
