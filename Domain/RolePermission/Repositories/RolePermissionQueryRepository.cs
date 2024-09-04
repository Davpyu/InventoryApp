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

        internal async Task<Models.RolePermission> Find(Guid id = default)
        {
            return await _context.RolePermissions.Include(x => new { x.Role, x.Permission }).Where(rolePermission => rolePermission.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Models.RolePermission> FindById(Guid id = default)
        {
            var rolePermission = await this.Find(id);
            if (rolePermission == null)
            {
                return null;
            }

            return rolePermission;
        }

        public async Task<Models.RolePermission> FindByRoleAndPermission(Guid roleid, Guid permission)
        {
            var rolePermission = await _context.RolePermissions
                .Where(rolePermission => rolePermission.Roleid == roleid && rolePermission.Permissionid == permission)
                .FirstAsync();

            if (rolePermission == null)
            {
                return null;
            }

            return rolePermission;
        }

        public async Task<List<Models.RolePermission>> FindByRoleId(Guid roleId)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rolePermission => rolePermission.Roleid == roleId)
                .ToListAsync();

            if (rolePermissions.Count < 1)
            {
                return [];
            }

            return rolePermissions;
        }
        
        public async Task<List<Models.RolePermission>> Get(int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.RolePermission> rolePermissions;
            rolePermissions = await _context.RolePermissions.Skip(skip).Take(perPage).ToListAsync();

            return rolePermissions;
        }

        public async Task<int> CountAll()
        {
            return await _context.RolePermissions.CountAsync();
        }
    }
}