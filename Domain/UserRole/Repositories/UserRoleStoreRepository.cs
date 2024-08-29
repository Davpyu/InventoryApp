using System.Data.Entity.Infrastructure;
using DotNetService.Exceptions;

namespace DotNetService.Domain.UserRole.Repositories
{
    public class UserRoleStoreRepository(
        UserRoleQueryRepository userRoleQueryRepository,
        Models.IamDBContext context
    )
    {
        private readonly UserRoleQueryRepository _userRoleQueryRepository = userRoleQueryRepository;
        private readonly Models.IamDBContext _context = context;

        public void Create(Models.UserRole userRole)
        {
            this.Save(userRole);
        }

        public void Update(Guid id, Models.UserRole userRole)
        {
            Models.UserRole oldUserRole = _userRoleQueryRepository.Find(id);
            if (oldUserRole == null)
            {
                return;
            }

            this.Save(userRole, true);
        }

        public void Delete(Guid id)
        {
            var userRole = _context.UserRoles.Where(userRole => userRole.Id == id).FirstOrDefault();
            _context.UserRoles.Remove(userRole);
            _context.SaveChanges();
        }

        public void DeleteBulk(List<Models.UserRole> data)
        {
            _context.UserRoles.RemoveRange(data);
            _context.SaveChanges();
        }

        private void Save(Models.UserRole data, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.UserRoles.Add(data);
                _context.SaveChanges();
            }
            try
            {
                var dataUpdated = _context.UserRoles.Update(data);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException(
                    "No data was updated."
                );
            }
        }

        public void BulkSave(Models.UserRole[] data)
        {
            _context.UserRoles.AddRange(data);
            _context.SaveChanges();
        }
    }
}
