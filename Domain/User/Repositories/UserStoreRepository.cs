using Microsoft.EntityFrameworkCore;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetService.Domain.User.Repositories
{
    public class UserStoreRepository(
        Models.IamDBContext context
    )
    {
        private readonly Models.IamDBContext _context = context;

        public async Task<Models.User> Create(Models.User data)
        {
            var dataCreated = _context.Users.Add(data);
            await _context.SaveChangesAsync();
            return dataCreated.Entity;
        }

        public async Task<Models.User> Update(Guid id, Models.User newData)
        {
            newData.Id = id;
            try
            {
                var dataUpdated = _context.Users.Update(newData);
                await _context.SaveChangesAsync();

                return dataUpdated.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was updated.");
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                Models.User data = new() { Id = id };
                _context.Users.Attach(data);
                _context.Users.Remove(data);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }
    }
}