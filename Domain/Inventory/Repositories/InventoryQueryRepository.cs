using System.Linq.Expressions;
using DotNetService.Domain.Inventory.Dtos;
using DotNetService.Infrastructure.Dtos;
using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Domain.Inventory.Repositories
{
    public partial class InventoryQueryRepository(
        IamDBContext context
        )
    {
        private readonly IamDBContext _context = context;

        public async Task<PaginationResult<Models.Inventory>> Pagination(InventoryQueryDto queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Inventories
            .AsNoTracking()
            .AsQueryable();

            query = QuerySearch(query, queryParams);
            // query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            var data = await query.Skip(skip).Take(queryParams.PerPage).ToListAsync();
            var count = await Count(query);

            return new PaginationResult<Models.Inventory>
            {
                Data = data,
                Count = count,
            };
        }

        private static IQueryable<Models.Inventory> QuerySearch(IQueryable<Models.Inventory> query, InventoryQueryDto queryParams)
        {
            if (queryParams.Search != null)
            {
                query = query.Where(data =>
                    data.Name.Contains(queryParams.Search)
                );
            }

            return query;
        }

        // private static IQueryable<Models.Inventory> QueryFilter(IQueryable<Models.Inventory> query, InventoryQueryDto queryParams)
        // {
        //     if (queryParams.Status != null)
        //     {
        //         query = query.Where(data => data.Status.Equals(queryParams.Status));
        //     }

        //     return query;
        // }

        private static IQueryable<Models.Inventory> QuerySort(IQueryable<Models.Inventory> query, InventoryQueryDto queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.Inventory, object>>> sortFunctions = new()
            {
                { "updated_at", data => data.UpdatedAt! },
                { "created_at", data => data.CreatedAt! },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.Inventory, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: {string.Join(", ", sortFunctions.Keys)}");
            }

            query = queryParams.Order == SortOrder.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> Count(IQueryable<Models.Inventory> query)
        {
            return await query.Select(x => x.Id).CountAsync();
        }
    }

    public partial class InventoryQueryRepository
    {

        public async Task<Models.Inventory> FindOneById(Guid id = default)
        {
            return await _context.Inventories
                .Where(data => data.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsExistByName(string name)
        {
            return await _context.Inventories.Where(inventory => inventory.Name == name).AnyAsync();
        }
    }
}