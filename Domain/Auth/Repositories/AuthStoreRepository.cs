namespace DotNetService.Domain.Auth.Repositories
{
    public class AuthStoreRepository
    {
        private readonly Models.IamDBContext _context;

        public AuthStoreRepository(
            Models.IamDBContext context
        )
        {
            _context = context;
        }

        public async Task Create(Models.User data)
        {
            _context.Users.Add(data);
            await _context.SaveChangesAsync();
        }
    }
}