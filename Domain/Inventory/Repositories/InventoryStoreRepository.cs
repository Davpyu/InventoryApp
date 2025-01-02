using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetService.Domain.Inventory.Repositories
{
    public class InventoryStoreRepository(
        IamDBContext context
    )
    {
        private readonly IamDBContext _context = context;

        public async Task Create(Models.Inventory data)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var newInventory = await _context.Inventories.AddAsync(new Models.Inventory
                {
                    Id = Guid.NewGuid(),
                    Name = data.Name,
                    Quantity = data.Quantity
                });
                var createdInventory = newInventory.Entity;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task Update(Guid id, Models.Inventory newData)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Update the inventory
                Models.Inventory data = new() { Id = id };
                _context.Inventories.Attach(data);
                _context.Inventories.Update(newData);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await _context.Database.RollbackTransactionAsync();
                throw new UnprocessableEntityException("No data was updated.");
            }
            catch (Exception)
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                Models.Inventory data = new() { Id = id };
                _context.Inventories.Attach(data);
                _context.Inventories.Remove(data);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }
    }
}