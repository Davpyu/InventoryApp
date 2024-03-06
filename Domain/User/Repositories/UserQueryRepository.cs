using System.Data.Entity;
using DotNetService.Exceptions;
using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1.User;

namespace DotNetService.Domain.User.Repositories
{
    public partial class UserQueryRepository(
        Models.IamDBContext context
        )
    {
        private readonly Models.IamDBContext _context = context;

        public List<Models.User> Pagination(UserQueryRequest queryParams)
        {
            int skip = (1 - queryParams.Page) * queryParams.PerPage;
            var query = _context.Users.AsQueryable().Include(user => user.UserRoles);

            query = this.QuerySearch(query, queryParams);
            query = this.QueryFilter(query, queryParams);
            query = this.QuerySort(query, queryParams);

            var data = query.Skip(skip).Take(queryParams.PerPage).ToList();

            return data;
        }

        private IQueryable<Models.User> QuerySearch(IQueryable<Models.User> query, UserQueryRequest queryParams)
        {
            if (queryParams.Search != null)
            {
                query = query.Where(user =>
                    user.Name.Contains(queryParams.Search) ||
                    user.Email.Contains(queryParams.Search));
            }

            return query;
        }

        private IQueryable<Models.User> QueryFilter(IQueryable<Models.User> query, UserQueryRequest queryParams)
        {
            // EXAMPLE: filter by email
            if (queryParams.Email != null)
            {
                query = query.Where(user => user.Email.Equals(queryParams.Email));
            }

            return query;
        }

        private IQueryable<Models.User> QuerySort(IQueryable<Models.User> query, UserQueryRequest queryParams)
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

        public int Count(UserQueryRequest queryParams)
        {
            IQueryable<Models.User> query = _context.Users;

            query = this.QuerySearch(query, queryParams);
            query = this.QueryFilter(query, queryParams);

            return query.Count();
        }
    }

    public partial class UserQueryRepository
    {

        internal Models.User Find(Guid id = default)
        {
            return _context.Users.Where(data => data.Id == id).FirstOrDefault();
        }

        public Models.User FindById(Guid id = default)
        {
            var data = this.Find(id);
            if (data == null)
            {
                return null;
            }

            return data;
        }

        public Models.User FindByEmail(string email, bool isValidateExist = false)
        {
            var data = _context.Users.Where(data => data.Email == email).FirstOrDefault();
            if (data == null)
            {
                return null;
            }

            if (isValidateExist && data != null)
            {
                throw new UnprocessableEntityException("email " + email + " has been used.");
            }

            return data;
        }        
    }
}