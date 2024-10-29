using System.Data.Entity;

namespace DotNetService.Domain.Auth.Repositories
{
    public class AuthQueryRepository(
        Models.IamDBContext context
        )
    {
        private readonly Models.IamDBContext _context = context;

        public async Task<Models.User> FindOneById(Guid id)
        {
            return await _context.Users.SingleOrDefaultAsync(data => data.Id.Equals(id));
        }

        public async Task<Models.User> FindOneByEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(data => data.Email.Equals(email));
        }

        public async Task<bool> IsEmailExist(string email)
        {
            return await _context.Users.AnyAsync(data => data.Email.Equals(email));
        }
    }
}