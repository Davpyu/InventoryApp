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

        public async Task Update(Guid id, UserRoleUpdateRequest userUpdate)
        {
            var updateUserRole = new Models.UserRole
            {
                Roleid = userUpdate.Roleid,
                Userid = userUpdate.Userid
            };

            await _userRoleStoreRepository.Update(id, updateUserRole);
        }

        public async Task Create(UserRoleCreateRequest userUpdate)
        {
            var newUserRole = new Models.UserRole
            {
                Roleid = userUpdate.Roleid,
                Userid = userUpdate.Userid
            };

            await _userRoleStoreRepository.Create(newUserRole);
        }

        public async Task Delete(Guid id)
        {
            await _userRoleStoreRepository.Delete(id);
        }

        public async Task<Models.UserRole> DetailById(Guid id)
        {
            return await _userRoleQueryRepository.FindById(id);
        }

        public async Task<List<Models.UserRole>> FindByUserId(Guid userId)
        {
            return await _userRoleQueryRepository.FindByUserId(userId);
        }

        public async Task<List<Models.UserRole>> GetList(int page, int perPage)
        {
            return await _userRoleQueryRepository.Get(page, perPage);
        }

        public async Task<int> Count(string search)
        {
            return await _userRoleQueryRepository.Count(search);
        }
    }
}