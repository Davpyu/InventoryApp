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

        public void Create(Models.Role roleRepository)
        {
            Models.Role newRole = new()
            {
                Name = roleRepository.Name
            };

            this.Save(newRole);
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
            Models.Role role = _context.Roles.Where(role => role.Id == id).FirstOrDefault();
            _context.Roles.Remove(role);
            _context.SaveChanges();
        }

        private void Save(Models.Role Role, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.Roles.Add(Role);
            }

            _context.SaveChanges();
        }
    }
}