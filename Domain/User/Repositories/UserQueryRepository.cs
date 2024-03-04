using System.Data.Entity;

namespace DotNetService.Domain.User.Repositories
{
    public class UserQueryRepository
    {
        private readonly Models.MainDBContext _context;

        public UserQueryRepository(
            Models.MainDBContext context
        )
        {
            _context = context;
        }

        internal Models.User Find(Guid id = default)
        {
            return _context.Users.Where(user => user.Id == id).FirstOrDefault();
        }

        public Models.User FindById(Guid id = default)
        {
            var user = this.Find(id);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public Models.User FindByEmail(string email = "")
        {
            var user = _context.Users.Where(user => user.Email == email).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            
            return user;
        }


        public List<Models.User> Get(string search, int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            var userQuery = _context.Users.AsQueryable().Include(x => x.UserRoles);
            if (search != null)
            {
                userQuery = userQuery.Where(user => user.Name.Contains(search));
            }
            var users = userQuery
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(perPage)
                .ToList();

            return users;
        }

        public int CountAll(string search)
        {
            IQueryable<Models.User> userQuery = _context.Users;
            if (search != null)
            {
                userQuery = userQuery.Where(user => user.Name.Contains(search));
            }
            return userQuery.Count();
        }
    }
}