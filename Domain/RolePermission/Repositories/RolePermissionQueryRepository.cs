using System.Data.Entity;

namespace DotNetService.Domain.RolePermission.Repositories
{
    public class RolePermissionQueryRepository
    {
        private readonly Models.IamDBContext _context;

        public RolePermissionQueryRepository(
            Models.IamDBContext context
        )
        {
            _context = context;
        }

        internal Models.RolePermission Find(Guid id = default)
        {
            return _context.RolePermissions.Include(x => new { x.Role, x.Permission }).Where(rolePermission => rolePermission.Id == id).FirstOrDefault();
        }

        public Models.RolePermission FindById(Guid id = default)
        {
            var rolePermission = this.Find(id);
            if (rolePermission == null)
            {
                return null;
            }

            return rolePermission;
        }

        public Models.RolePermission FindByRoleAndPermission(Guid roleid, Guid permission)
        {
            var rolePermission = _context.RolePermissions
                .Where(rolePermission => rolePermission.Roleid == roleid && rolePermission.Permissionid == permission)
                .First();

            if (rolePermission == null)
            {
                return null;
            }

            return rolePermission;
        }

        public List<Models.RolePermission> FindByRoleId(Guid roleId)
        {
            var rolePermissions = _context.RolePermissions
                .Where(rolePermission => rolePermission.Roleid == roleId)
                .ToList();

            if (rolePermissions.Count < 1)
            {
                return [];
            }

            return rolePermissions;
        }
        
        public List<Models.RolePermission> Get(int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.RolePermission> rolePermissions;
            rolePermissions = _context.RolePermissions.Skip(skip).Take(perPage).ToList();

            return rolePermissions;
        }

        public int CountAll()
        {
            return _context.RolePermissions.Count();
        }
    }
}