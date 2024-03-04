using System.Data.Entity;

namespace DotNetService.Domain.UserRole.Repositories
{
    public class UserRoleQueryRepository
    {
        private readonly Models.MainDBContext _context;

        internal Models.UserRole Find(Guid id = default)
        {
            return _context.UserRoles.Include("User").Include("Role").Where(userRole => userRole.Id == id).FirstOrDefault();
        }

        public Models.UserRole FindById(Guid id = default)
        {
            var userRole = this.Find(id);
            if (userRole == null)
            {
                return null;
            }

            return userRole;
        }

        public Models.UserRole FindByUserAndRole(Guid userid, Guid roleid)
        {
            Models.UserRole userRole = _context.UserRoles.Where(userRole => userRole.Userid == userid && userRole.Roleid == roleid).FirstOrDefault();
            if (userRole == null)
            {
                return null;
            }

            return userRole;
        }

        public List<Models.UserRole> FindByUserId(Guid userId = default)
        {
            var userRoles = _context.UserRoles.Where(userRole => userRole.Userid == userId).ToList();
            if (userRoles.Count < 1)
            {
                return [];
            }

            return userRoles;
        }

        public List<Models.UserRole> Get(int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.UserRole> userRoles;
            IQueryable<Models.UserRole> userQuery = _context.UserRoles;
            userRoles = userQuery.Skip(skip).Take(perPage).ToList();
            
            return userRoles;
        }

        public int Count(string search)
        {
            return _context.UserRoles
            .Include(
                x => new { x.Role, x.User }
            )
            .Where(
                x => x.Role.Name.Contains(search) || x.User.Name.Contains(search)
            ).Count();
        }
    }
}