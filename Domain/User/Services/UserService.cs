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
            Models.User data = new()
            {
                Name = authRegister.Name,
                Email = authRegister.Email,
                Password = BC.HashPassword(authRegister.Password)
            };

            _userStoreRepository.Create(data);
        }

        public void Create(UserCreate userCreate)
        {
            Models.User data = new()
            {
                Name = userCreate.Name
            };

            _userStoreRepository.Create(data);
        }

        public void Update(Guid id, UserUpdate userUpdate)
        {
            Models.User data = new()
            {
                Name = userUpdate.Name
            };

            _userStoreRepository.Update(id, data);
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