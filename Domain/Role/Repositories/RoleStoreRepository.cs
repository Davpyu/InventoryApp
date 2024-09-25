using System.Data.Entity.Infrastructure;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetService.Domain.Role.Repositories
{
    public class RoleStoreRepository
    {
        private readonly RoleQueryRepository _roleQueryRepository;
        private readonly Models.IamDBContext _context;

        public RoleStoreRepository(
            Models.IamDBContext context,
            RoleQueryRepository roleQueryRepository
        )
        {
            _context = context;
            _roleQueryRepository = roleQueryRepository;
        }

        public async Task<Models.Role> Create(Models.Role role)
        {
            Models.Role newRole = new()
            {
                Name = role.Name
            };

            return await this.Save(newRole);
        }

        public async Task Update(Guid id, Models.Role roleRepository)
        {
            Models.Role oldRole = await _roleQueryRepository.Find(id);
            if (oldRole == null)
            {
                return;
            }

            oldRole.Name = roleRepository.Name;
            await this.Save(oldRole, true);
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

        private async Task<Models.Role> Save(Models.Role data, bool isUpdate = false)
        {
            if (!isUpdate)
            {

                var dataCreated = _context.Roles.Add(data);
                await _context.SaveChangesAsync();
                return dataCreated.Entity;
            }

            try
            {
                var dataUpdated = _context.Roles.Update(data);
                await _context.SaveChangesAsync();

                return dataUpdated.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was updated.");
            }
        }
    }
}