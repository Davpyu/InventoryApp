using System.Data.Entity;
using DotNetService.Http.API.Version1.Requests;
using DotNetService.Http.API.Version1.Requests.User;

namespace DotNetService.Domain.User.Repositories
{
    public partial class UserQueryRepository
    {
        private readonly Models.IamDBContext _context;

        public UserQueryRepository(
            Models.IamDBContext context
        )
        {
            _context = context;
        }

        public List<Models.User> Pagination(UserQuery queryParams)
        {
            int skip = (1 - queryParams.Page) * queryParams.PerPage;
            var query = _context.Users.AsQueryable().Include(user => user.UserRoles);

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            List<Models.User> users = query.Skip(skip).Take(queryParams.PerPage).ToList();

            return users;
        }

        private IQueryable<Models.User> QuerySearch(IQueryable<Models.User> query, UserQuery queryParams)
        {
            if (queryParams.Search != null)
            {
                query = query.Where(user =>
                    user.Name.Contains(queryParams.Search) ||
                    user.Email.Contains(queryParams.Search));
            }

            return query;
        }

        private IQueryable<Models.User> QueryFilter(IQueryable<Models.User> query, UserQuery queryParams)
        {
            // EXAMPLE: filter by email
            if (queryParams.Email != null)
            {
                query = query.Where(user => user.Email.Equals(queryParams.Email));
            }

            return query;
        }

        private IQueryable<Models.User> QuerySort(IQueryable<Models.User> query, UserQuery queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Func<Models.User, object>> sortFunctions = new()
            {
                { "name", user => user.Name },
                { "email", user => user.Email },
                { "updated_at", user => user.UpdatedAt },
                { "created_at", user => user.CreatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Func<Models.User, object> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrderEnum.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }
    }

    public partial class UserQueryRepository
    {

        internal Models.User Find(Guid id = default)
        {
            return _context.Users.Where(user => user.Id == id).FirstOrDefault();
        }

        public Models.User FindById(Guid id = default)
        {
            var user = this.Find(id);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public Models.User FindByEmail(string email = "")
        {
            var user = _context.Users.Where(user => user.Email == email).FirstOrDefault();
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public int CountAll(string search)
        {
            IQueryable<Models.User> userQuery = _context.Users;
            if (search != null)
            {
                userQuery = userQuery.Where(user => user.Name.Contains(search));
            }
            return userQuery.Count();
        }
    }
}