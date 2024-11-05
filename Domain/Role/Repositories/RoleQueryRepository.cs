using System.Linq.Expressions;
using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1.Role;
using DotNetService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Domain.Role.Repositories
{
    public class RoleQueryRepository(
        Models.IamDBContext context
        )
    {
        private readonly Models.IamDBContext _context = context;

        public async Task<List<Models.Role>> Pagination(RoleQueryRequest queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Roles
                .Include(data => data.RolePermissions)
                .ThenInclude(data => data.Permission)
                .AsNoTracking()
                .AsQueryable();

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            var data = await query.Skip(skip).Take(queryParams.PerPage).ToListAsync();

            return data;
        }

        private static IQueryable<Models.Role> QuerySearch(IQueryable<Models.Role> query, RoleQueryRequest queryParams)
        {
            if (queryParams.Search != null)
            {
                query = query.Where(data =>
                    data.Name.Contains(queryParams.Search));
            }

            return query;
        }

        private static IQueryable<Models.Role> QueryFilter(IQueryable<Models.Role> query, RoleQueryRequest queryParams)
        {
            if (queryParams.Name != null)
            {
                query = query.Where(data => data.Name.Equals(queryParams.Name));
            }

            return query;
        }

        private static IQueryable<Models.Role> QuerySort(IQueryable<Models.Role> query, RoleQueryRequest queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.Role, object>>> sortFunctions = new()
            {
                { "name", data => data.Name },
                { "updated_at", data => data.UpdatedAt },
                { "created_at", data => data.CreatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.Role, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrderEnum.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> Count(RoleQueryRequest queryParams)
        {
            IQueryable<Models.Role> query = _context.Roles.AsNoTracking();

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);

            return await query.Select(x => x.Id).CountAsync();
        }

        public async Task<Models.Role> FindOneById(Guid id = default, bool isThrowException = false)
        {
            var data = await _context.Roles
                .Where(data => data.Id == id)
                .Include(data => data.RolePermissions)
                .ThenInclude(data => data.Permission)
                .FirstOrDefaultAsync();

            if (data == null && isThrowException)
            {
                throw new DataNotFoundException("Role with id " + id + " not found.");
            };

            return data;
        }

        public async Task<bool> IsExistByKey(string key)
        {
            return await _context.Roles.Where(role => role.Key == key).AnyAsync();
        }

        public async Task<List<Models.Role>> Get(string search, int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.Role> roles;
            IQueryable<Models.Role> roleQuery = _context.Roles;
            if (search != null)
            {
                roleQuery = roleQuery.Where(role => role.Name.Contains(search));
            }
            roles = await roleQuery.Skip(skip).Take(perPage).ToListAsync();
            return roles;
        }

        public async Task<int> CountAll(string search)
        {
            IQueryable<Models.Role> roleQuery = _context.Roles;
            if (search != null)
            {
                roleQuery = roleQuery.Where(role => role.Name.Contains(search));
            }
            return await roleQuery.CountAsync();
        }
    }
}