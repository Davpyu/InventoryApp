using System.Data.Entity;

namespace DotNetService.Domain.Permission.Repositories
{
    public class PermissionQueryRepository
    {
        private readonly Models.MainDBContext _context;

        public PermissionQueryRepository(
            Models.MainDBContext context
        )
        {
            _context = context;
        }

        internal Models.Permission Find(Guid id = default)
        {
            return _context.Permissions.Where(permission => permission.Id == id).FirstOrDefault();
        }

        public Models.Permission FindById(Guid id = default)
        {
            Models.Permission permission = this.Find(id);
            if (permission == null)
            {
                return null;
            }

            return permission;
        }

        public Models.Permission FindByName(string name)
        {
            Models.Permission permission = _context.Permissions.Where(permission => permission.Name == name).FirstOrDefault();
            if (permission == null)
            {
                return null;
            }

            return permission;
        }

        public List<Models.Permission> Get(string search, int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.Permission> permissions;
            IQueryable<Models.Permission> permissionQuery = _context.Permissions;
            if (search != null)
            {
                permissionQuery = permissionQuery.Where(permission => permission.Name.Contains(search));
            }
            permissions = permissionQuery.Skip(skip).Take(perPage).ToList();

            return permissions;
        }

        public int CountAll(string search)
        {
            IQueryable<Models.Permission> permissionQuery = _context.Permissions;
            if (search != null)
            {
                permissionQuery = permissionQuery.Where(permission => permission.Name.Contains(search));
            }
            
            return permissionQuery.Count();
        }
    }
}