using System.Data.Entity;
using DotNetService.Http.API.Version1;

namespace DotNetService.Domain.UserRole.Repositories
{
    public class UserRoleQueryRepository(
        Models.IamDBContext context
        )
    {
        private readonly Models.IamDBContext _context = context;

        internal async Task<Models.UserRole> Find(Guid id = default)
        {
            return await _context.UserRoles.Include("User").Include("Role").Where(userRole => userRole.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Models.UserRole> FindById(Guid id = default)
        {
            var userRole = await this.Find(id);
            if (userRole == null)
            {
                return null;
            }

            return userRole;
        }

        public async Task<Models.UserRole> FindByUserAndRole(Guid userid, Guid roleid)
        {
            Models.UserRole userRole = _context.UserRoles.Where(userRole => userRole.UserId == userid && userRole.RoleId == roleid).FirstOrDefaultAsync();
            if (userRole == null)
            {
                return null;
            }

            return userRole;
        }

        public async Task<List<Models.UserRole>> FindByUserId(Guid userId = default)
        {
            var userRoles = await _context.UserRoles.Where(userRole => userRole.Userid == userId).ToListAsync();
            if (userRoles.Count < 1)
            {
                return [];
            }

            return userRoles;
        }

        public async Task<List<Models.UserRole>> Get(int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.UserRole> userRoles;
            IQueryable<Models.UserRole> userQuery = _context.UserRoles;
            userRoles = await userQuery.Skip(skip).Take(perPage).ToListAsync();

            return userRoles;
        }

        public async Task<int> Count(string search)
        {
            IQueryable<Models.UserRole> query = _context
                .UserRoles.Include(x => x.Role)
                .Include(x => x.User);

            query = QuerySearch(query, new Query { Search = search });

            return await query.CountAsync();
        }

        private static IQueryable<Models.UserRole> QuerySearch(
            IQueryable<Models.UserRole> query,
            Query queryParams
        )
        {
            if (queryParams.Search != null)
            {
                query = query.Where(x =>
                    x.Role.Name.Contains(queryParams.Search)
                    || x.User.Name.Contains(queryParams.Search)
                );
            }

            return query;
        }
    }
}
