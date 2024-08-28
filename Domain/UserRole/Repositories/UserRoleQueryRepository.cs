using System.Data.Entity;
using DotNetService.Http.API.Version1;

namespace DotNetService.Domain.UserRole.Repositories
{
    public class UserRoleQueryRepository(
        Models.IamDBContext context
        )
    {
        private readonly Models.IamDBContext _context = context;

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
            IQueryable<Models.UserRole> query = _context
                .UserRoles.Include(x => x.Role)
                .Include(x => x.User);

            query = QuerySearch(query, new Query { Search = search });

            return query.Count();
        }

        private static  IQueryable<Models.UserRole> QuerySearch(
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
