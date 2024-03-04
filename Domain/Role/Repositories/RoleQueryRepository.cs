using System.Data.Entity;

namespace DotNetService.Domain.Role.Repositories
{
    public class RoleQueryRepository
    {
        private readonly Models.MainDBContext _context;

        public RoleQueryRepository(
            Models.MainDBContext context
        )
        {
            _context = context;
        }

        internal Models.Role Find(Guid id = default)
        {
            return _context.Roles.Where(role => role.Id == id).FirstOrDefault();
        }

        public Models.Role FindById(Guid id = default)
        {
            Models.Role role = this.Find(id);
            if (role == null)
            {
                return null;
            }

            return role;
        }

        public Models.Role FindByName(string name)
        {
            Models.Role role = _context.Roles.Where(role => role.Name == name).FirstOrDefault();
            if (role == null)
            {
                return (new Models.Role());
            }

            return role;
        }

        public bool IsExistsByNameAndIds(string nameRole, Guid[] roleIds)
        {
            return _context.Roles.Where(role => role.Name == nameRole).Where(role => roleIds.Contains(role.Id)).Count() > 0;
        }

        public List<Models.Role> Get(string search, int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.Role> roles;
            IQueryable<Models.Role> roleQuery = _context.Roles;
            if (search != null)
            {
                roleQuery = roleQuery.Where(role => role.Name.Contains(search));
            }
            roles = roleQuery.Skip(skip).Take(perPage).ToList();
            return roles;
        }

        public int CountAll(string search)
        {
            IQueryable<Models.Role> roleQuery = _context.Roles;
            if (search != null)
            {
                roleQuery = roleQuery.Where(role => role.Name.Contains(search));
            }
            return roleQuery.Count();
        }
    }
}