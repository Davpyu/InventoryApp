using System.Linq.Expressions;
using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1.Permission;
using DotNetService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Domain.Permission.Repositories
{
    public class PermissionQueryRepository(
        Models.IamDBContext context
        )
    {
        private readonly Models.IamDBContext _context = context;

        public async Task<List<Models.Permission>> Pagination(PermissionQueryRequest queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Permissions
                .Include(data => data.RolePermissions)
                .AsQueryable();

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            var data = await query.Skip(skip).Take(queryParams.PerPage).ToListAsync();

            return data;
        }

        private static IQueryable<Models.Permission> QuerySearch(IQueryable<Models.Permission> query, PermissionQueryRequest queryParams)
        {
            if (queryParams.Search != null)
            {
                query = query.Where(data =>
                    data.Name.Contains(queryParams.Search));
            }

            return query;
        }

        private IQueryable<Models.Permission> QueryFilter(IQueryable<Models.Permission> query, PermissionQueryRequest queryParams)
        {
            if (queryParams.Name != null)
            {
                query = query.Where(data => data.Name.Equals(queryParams.Name));
            }

            return query;
        }

        private IQueryable<Models.Permission> QuerySort(IQueryable<Models.Permission> query, PermissionQueryRequest queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.Permission, object>>> sortFunctions = new()
            {
                { "name", data => data.Name },
                { "updated_at", data => data.UpdatedAt },
                { "created_at", data => data.CreatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.Permission, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrderEnum.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> Count(PermissionQueryRequest queryParams)
        {
            IQueryable<Models.Permission> query = _context.Permissions;

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);

            return await query.CountAsync();
        }

        internal async Task<Models.Permission> Find(Guid id = default)
        {
            return await _context.Permissions.Where(permission => permission.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Models.Permission> FindOneById(Guid id = default, bool isThrowException = false)
        {
            var data = await _context.Permissions
                .Where(data => data.Id == id)
                .FirstOrDefaultAsync();

            if (data == null && isThrowException)
            {
                throw new DataNotFoundException("Role with id " + id + " not found.");
            };

            return data;
        }

        public async Task<Models.Permission> FindByName(string name)
        {
            Models.Permission permission = await _context.Permissions.Where(permission => permission.Name == name).FirstOrDefaultAsync();
            if (permission == null)
            {
                return null;
            }

            return permission;
        }

        public async Task<List<Models.Permission>> Get(string search, int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.Permission> permissions;
            IQueryable<Models.Permission> permissionQuery = _context.Permissions;
            if (search != null)
            {
                permissionQuery = permissionQuery.Where(permission => permission.Name.Contains(search));
            }
            permissions = await permissionQuery.Skip(skip).Take(perPage).ToListAsync();

            return permissions;
        }

        public async Task<int> CountAll(string search)
        {
            IQueryable<Models.Permission> permissionQuery = _context.Permissions;
            if (search != null)
            {
                permissionQuery = permissionQuery.Where(permission => permission.Name.Contains(search));
            }

            return await permissionQuery.CountAsync();
        }
    }
}