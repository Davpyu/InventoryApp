using Models = DotNetService.Models;
using System.Linq;
using DotNetService.Exceptions;

namespace DotNetService.Domain.User.Repositories
{
    public class UserStoreRepository
    {
        private readonly Models.IamDBContext _context;
        private readonly UserQueryRepository _userQueryRepository;

        public UserStoreRepository(
            Models.IamDBContext context,
            UserQueryRepository userQueryRepository
        )
        {
            _context = context;
            _userQueryRepository = userQueryRepository;
        }

        public void Create(Models.User newUser)
        {
            this.Save(newUser);
        }

        public void Update(Guid id, Models.User user)
        {
            Models.User oldUser = _userQueryRepository.Find(id) ??
            throw new DataNotFoundException("User id " + id + " not found");

            oldUser.Name = user.Name;

            this.Save(oldUser, true);
        }

        public void Delete(Guid id)
        {
            Models.User oldUser = _userQueryRepository.Find(id) ??
            throw new DataNotFoundException("User id " + id + " not found");
            
            Models.User user = _context.Users.Where(user => user.Id == id).FirstOrDefault();
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        private void Save(Models.User User, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.Users.Add(User);
            }

            _context.SaveChanges();
        }
    }
}