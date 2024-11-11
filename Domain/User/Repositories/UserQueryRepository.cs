using System.Linq.Expressions;
using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1.User;
using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Domain.User.Repositories
{
    public partial class UserQueryRepository(
        IamDBContext context
        )
    {
        private readonly IamDBContext _context = context;

        public async Task<List<Models.User>> Pagination(UserQueryRequest queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Users
            .Include(data => data.UserRoles)
            .ThenInclude(data => data.Role)
            .AsQueryable();

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            var data = await query.Skip(skip).Take(queryParams.PerPage).ToListAsync();

            return data;
        }

        private static IQueryable<Models.User> QuerySearch(IQueryable<Models.User> query, UserQueryRequest queryParams)
        {
            if (queryParams.Search != null)
            {
                query = query.Where(data =>
                    data.Name.Contains(queryParams.Search) ||
                    data.Email.Contains(queryParams.Search)
                );
            }

            return query;
        }

        private static IQueryable<Models.User> QueryFilter(IQueryable<Models.User> query, UserQueryRequest queryParams)
        {
            // EXAMPLE: filter by email
            if (queryParams.Email != null)
            {
                query = query.Where(data => data.Email.Equals(queryParams.Email));
            }

            return query;
        }

        private static IQueryable<Models.User> QuerySort(IQueryable<Models.User> query, UserQueryRequest queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.User, object>>> sortFunctions = new()
            {
                { "name", data => data.Name },
                { "email", data => data.Email },
                { "updated_at", data => data.UpdatedAt },
                { "created_at", data => data.CreatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.User, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrderEnum.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> Count(UserQueryRequest queryParams)
        {
            IQueryable<Models.User> query = _context.Users;

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);

            return await query.CountAsync();
        }
    }

    public partial class UserQueryRepository
    {

        public async Task<Models.User> FindOneById(Guid id = default, bool isThrowException = false)
        {
            var data = await _context.Users
                .Where(data => data.Id == id)
                .Include(data => data.UserRoles)
                .ThenInclude(data => data.Role)
                .ThenInclude(data => data.RolePermissions)
                .ThenInclude(data => data.Permission)
                .FirstOrDefaultAsync();

            if (data == null && isThrowException)
            {
                throw new DataNotFoundException("User with id " + id + " not found.");
            };

            return data;
        }

        public async Task<Models.User> FindOneByEmail(string email)
        {
            return await _context.Users.Where(data => data.Email == email).SingleOrDefaultAsync();
        }

        public async Task<bool> IsEmailExists(string email)
        {
            return await _context.Users.AnyAsync(data => data.Email == email);
        }

        public async Task<bool> IsEmailExistsExceptId(string email, Guid id)
        {
            return await _context.Users.AnyAsync(data => data.Email == email && data.Id != id);
        }
    }
}