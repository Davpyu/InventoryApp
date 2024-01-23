using DotNetService.Http.API.Version1.Requests.User;
using DotNetService.Http.API.Version1.Requests.Auth;
using BC = BCrypt.Net.BCrypt;
using DotNetService.Domain.User.Repositories;

namespace DotNetService.Domain.User.Services
{
    public class UserService(
            UserQueryRepository userQueryRepository,
            UserStoreRepository userStoreRepository
        )
    {
        private readonly UserQueryRepository _userQueryRepository = userQueryRepository;
        private readonly UserStoreRepository _userStoreRepository = userStoreRepository;

        public void Register(AuthRegister authRegister)
        {
            var userRepository = new Models.User
            {
                Name = authRegister.Name,
                Email = authRegister.Email,
                Password = BC.HashPassword(authRegister.Password)
            };

            _userStoreRepository.Create(userRepository);
        }

        public void Update(Guid id, UserUpdate userUpdate)
        {
            Models.User userRepository = new Models.User
            {
                Name = userUpdate.Name
            };

            _userStoreRepository.Update(id, userRepository);
        }

        public void Delete(Guid id)
        {
            _userStoreRepository.Delete(id);
        }

        public Models.User DetailById(Guid id)
        {
            return _userQueryRepository.FindById(id);
        }

        public List<Models.User> GetList(string search, int page, int perPage)
        {
            return _userQueryRepository.Get(search, page, perPage);
        }

        public int Count(string search)
        {
            return _userQueryRepository.CountAll(search);
        }
    }
}