using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1.Permission;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Domain.Permission.Repositories
{
    public class PermissionQueryRepository(
        Models.IamDBContext context
        )
    {
        private readonly Models.IamDBContext _context = context;

        public List<Models.Permission> Pagination(PermissionQueryRequest queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Permissions
                .Include(data => data.RolePermissions)
                .AsQueryable();

            query = this.QuerySearch(query, queryParams);
            query = this.QueryFilter(query, queryParams);
            query = this.QuerySort(query, queryParams);

            var data = query.Skip(skip).Take(queryParams.PerPage).ToList();

            return data;
        }

        private IQueryable<Models.Permission> QuerySearch(IQueryable<Models.Permission> query, PermissionQueryRequest queryParams)
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

            Dictionary<string, Func<Models.Permission, object>> sortFunctions = new()
            {
                { "name", data => data.Name },
                { "updated_at", data => data.UpdatedAt },
                { "created_at", data => data.CreatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Func<Models.Permission, object> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrderEnum.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public int Count(PermissionQueryRequest queryParams)
        {
            IQueryable<Models.Permission> query = _context.Permissions;

            query = this.QuerySearch(query, queryParams);
            query = this.QueryFilter(query, queryParams);

            return query.Count();
        }

        internal Models.Permission Find(Guid id = default)
        {
            return _context.Permissions.Where(permission => permission.Id == id).FirstOrDefault();
        }

        public Models.Permission FindById(Guid id = default)
        {
            Models.Permission permission = this.Find(id);
            if (permission == null)
            {
                return null;
            }

            return permission;
        }

        public Models.Permission FindByName(string name)
        {
            Models.Permission permission = _context.Permissions.Where(permission => permission.Name == name).FirstOrDefault();
            if (permission == null)
            {
                return null;
            }

            return permission;
        }

        public List<Models.Permission> Get(string search, int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.Permission> permissions;
            IQueryable<Models.Permission> permissionQuery = _context.Permissions;
            if (search != null)
            {
                permissionQuery = permissionQuery.Where(permission => permission.Name.Contains(search));
            }
            permissions = permissionQuery.Skip(skip).Take(perPage).ToList();

            return permissions;
        }

        public int CountAll(string search)
        {
            IQueryable<Models.Permission> permissionQuery = _context.Permissions;
            if (search != null)
            {
                permissionQuery = permissionQuery.Where(permission => permission.Name.Contains(search));
            }
            
            return permissionQuery.Count();
        }
    }
}