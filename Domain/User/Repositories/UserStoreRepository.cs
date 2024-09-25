using Microsoft.EntityFrameworkCore;
using DbDeleteConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace DotNetService.Domain.User.Repositories
{
    public class UserStoreRepository(
        Models.IamDBContext context,
        UserQueryRepository userQueryRepository
    )
    {
        private readonly Models.IamDBContext _context = context;
        private readonly UserQueryRepository _userQueryRepository = userQueryRepository;

        public async Task<Models.User> Create(Models.User data)
        {
            return await this.Save(data);
        }

        public async Task<Models.User> Update(Guid id, Models.User newData)
        {
            newData.Id = id;
            return await this.Save(newData, true);
        }

        public async Task Delete(Guid id)
        {
            try
            {
                Models.User data = new Models.User { Id = id };
                _context.Users.Attach(data);
                _context.Users.Remove(data);
                await _context.SaveChangesAsync();
            }
            catch (DbDeleteConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }

        private async Task<Models.User> Save(Models.User data, bool isUpdate = false)
        {

            if (!isUpdate)
            {
                await _userQueryRepository.FindOneByEmail(data.Email, true);

                var dataCreated = _context.Users.Add(data);
                await _context.SaveChangesAsync();
                return dataCreated.Entity;
            }

            try
            {
                var dataUpdated = _context.Users.Update(data);
                await _context.SaveChangesAsync();

                return dataUpdated.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was updated.");
            }
        }
    }
}