using DotNetService.Http.API.Version1.UserRole;
using DotNetService.Domain.UserRole.Repositories;

namespace DotNetService.Domain.UserRole.Services
{
    public class UserRoleService (
        UserRoleQueryRepository userRoleQueryRepository,
        UserRoleStoreRepository userRoleStoreRepository
        )
    {
        private readonly UserRoleQueryRepository _userRoleQueryRepository = userRoleQueryRepository;
        private readonly UserRoleStoreRepository _userRoleStoreRepository = userRoleStoreRepository;

        public void Update(Guid id, UserRoleUpdateRequest userUpdate)
        {
            var updateUserRole = new Models.UserRole
            {
                Roleid = userUpdate.Roleid,
                Userid = userUpdate.Userid
            };

            _userRoleStoreRepository.Update(id, updateUserRole);
        }

        public void Create(UserRoleCreateRequest userUpdate)
        {
            var newUserRole = new Models.UserRole
            {
                Roleid = userUpdate.Roleid,
                Userid = userUpdate.Userid
            };

            _userRoleStoreRepository.Create(newUserRole);
        }

        public void Delete(Guid id)
        {
            _userRoleStoreRepository.Delete(id);
        }

        public Models.UserRole DetailById(Guid id)
        {
            return _userRoleQueryRepository.FindById(id);
        }

        public List<Models.UserRole> FindByUserId(Guid userId)
        {
            return _userRoleQueryRepository.FindByUserId(userId);
        }

        public List<Models.UserRole> GetList(int page, int perPage)
        {
            return _userRoleQueryRepository.Get(page, perPage);
        }

        public int Count(string search)
        {
            return _userRoleQueryRepository.Count(search);
        }
    }
}