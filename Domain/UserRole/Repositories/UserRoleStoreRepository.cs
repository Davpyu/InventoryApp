namespace DotNetService.Domain.UserRole.Repositories
{
    public class UserRoleStoreRepository
    {
        private readonly UserRoleQueryRepository _userRoleQueryRepository;
        private readonly Models.DBContext1 _context;

        public void Create(Models.UserRole userRole)
        {
            var newUserRole = new Models.UserRole();
            this.Save(newUserRole);
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

        private void Save(Models.UserRole UserRole, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.UserRoles.Add(UserRole);
            }
            _context.SaveChanges();
        }
    }
}