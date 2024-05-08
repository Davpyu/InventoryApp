using DotNetService.Exceptions;
using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1.Role;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Domain.Role.Repositories
{
    public class RoleQueryRepository
    {
        private readonly Models.IamDBContext _context;

        public RoleQueryRepository(
            Models.IamDBContext context
        )
        {
            _context = context;
        }

        public List<Models.Role> Pagination(RoleQueryRequest queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Roles
                .Include(data => data.RolePermissions)
                .ThenInclude(data => data.Permission)
                .AsQueryable();

            query = this.QuerySearch(query, queryParams);
            query = this.QueryFilter(query, queryParams);
            query = this.QuerySort(query, queryParams);

            var data = query.Skip(skip).Take(queryParams.PerPage).ToList();

            return data;
        }

        private IQueryable<Models.Role> QuerySearch(IQueryable<Models.Role> query, RoleQueryRequest queryParams)
        {
            if (queryParams.Search != null)
            {
                query = query.Where(data =>
                    data.Name.Contains(queryParams.Search));
            }

            return query;
        }

        private IQueryable<Models.Role> QueryFilter(IQueryable<Models.Role> query, RoleQueryRequest queryParams)
        {
            if (queryParams.Name != null)
            {
                query = query.Where(data => data.Name.Equals(queryParams.Name));
            }

            return query;
        }

        private IQueryable<Models.Role> QuerySort(IQueryable<Models.Role> query, RoleQueryRequest queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Func<Models.Role, object>> sortFunctions = new()
            {
                { "name", data => data.Name },
                { "updated_at", data => data.UpdatedAt },
                { "created_at", data => data.CreatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Func<Models.Role, object> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrderEnum.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public int Count(RoleQueryRequest queryParams)
        {
            IQueryable<Models.Role> query = _context.Roles;

            query = this.QuerySearch(query, queryParams);
            query = this.QueryFilter(query, queryParams);

            return query.Count();
        }

        internal Models.Role Find(Guid id = default)
        {
            return _context.Roles.Where(role => role.Id == id).FirstOrDefault();
        }

        public Models.Role FindOneById(Guid id = default, bool isThrowException = false)
        {
            var data = _context.Roles
                .Where(data => data.Id == id)
                .Include(data => data.RolePermissions)
                .ThenInclude(data => data.Permission)
                .FirstOrDefault();

            if (data == null && isThrowException)
            {
                throw new DataNotFoundException("Role with id " + id + " not found.");
            };

            return data;
        }

        public Models.Role FindByName(string name)
        {
            Models.Role role = _context.Roles.Where(role => role.Name == name).FirstOrDefault();
            if (role == null)
            {
                return (new Models.Role());
            }

            return role;
        }

        public bool IsExistsByNameAndIds(string nameRole, Guid[] roleIds)
        {
            return _context.Roles.Where(role => role.Name == nameRole).Where(role => roleIds.Contains(role.Id)).Count() > 0;
        }

        public List<Models.Role> Get(string search, int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.Role> roles;
            IQueryable<Models.Role> roleQuery = _context.Roles;
            if (search != null)
            {
                roleQuery = roleQuery.Where(role => role.Name.Contains(search));
            }
            roles = roleQuery.Skip(skip).Take(perPage).ToList();
            return roles;
        }

        public int CountAll(string search)
        {
            IQueryable<Models.Role> roleQuery = _context.Roles;
            if (search != null)
            {
                roleQuery = roleQuery.Where(role => role.Name.Contains(search));
            }
            return roleQuery.Count();
        }
    }
}