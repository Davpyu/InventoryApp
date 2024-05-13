using System.Data.Entity.Infrastructure;
using DotNetService.Exceptions;

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

        public Models.Role Create(Models.Role role)
        {
            Models.Role newRole = new()
            {
                Name = role.Name
            };

            return this.Save(newRole);
        }

        public void Update(Guid id, Models.Role roleRepository)
        {
            Models.Role oldRole = _roleQueryRepository.Find(id);
            if (oldRole == null)
            {
                return;
            }

            oldRole.Name = roleRepository.Name;
            this.Save(oldRole, true);
        }

        public void Delete(Guid id)
        {
            Models.Role data = _roleQueryRepository.FindOneById(id, true);

            _context.Roles.Remove(data);
            int affectedRows = _context.SaveChanges();

            if (affectedRows == 0)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }

        private Models.Role Save(Models.Role data, bool isUpdate = false)
        {
            if (!isUpdate)
            {

                var dataCreated = _context.Roles.Add(data);
                _context.SaveChanges();
                return dataCreated.Entity;
            }

            try
            {
                var dataUpdated = _context.Roles.Update(data);
                int affectedRows = _context.SaveChanges();

                if (affectedRows == 0)
                {
                    throw new UnprocessableEntityException("No data was updated.");
                }

                return dataUpdated.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DataNotFoundException("User with id " + data.Id + " not found.");
            }
        }
    }
}