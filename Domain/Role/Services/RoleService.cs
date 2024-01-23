using DotNetService.Domain.Role.Repositories;
using DotNetService.Http.API.Version1.Requests.Role;

namespace DotNetService.Domain.Role.Services
{
    public class RoleService(
        RoleStoreRepository roleStoreRepository,
        RoleQueryRepository roleQueryRepository
        )
    {
        private readonly RoleStoreRepository _roleStoreRepository = roleStoreRepository;
        private readonly RoleQueryRepository _roleQueryRepository = roleQueryRepository;

        public void Create(RoleCreate roleCreate)
        {
            var roleRepository = new Models.Role
            {
                Name = roleCreate.Name
            };

            _roleStoreRepository.Create(roleRepository);
        }

        public Models.Role DetailById(Guid id)
        {
            return _roleQueryRepository.FindById(id);
        }

        public List<Models.Role> GetList(string search, int page, int perPage)
        {
            return _roleQueryRepository.Get(search, page, perPage);
        }
        public int Count(string search)
        {
            return _roleQueryRepository.CountAll(search);
        }

        public void Update(Guid id, RoleUpdate roleUpdate)
        {
            Models.Role roleRepository = new Models.Role();
            roleRepository.Name = roleUpdate.Name;
            _roleStoreRepository.Update(id, roleRepository);
        }

        public void Delete(Guid id)
        {
            _roleStoreRepository.Delete(id);
        }
    }
}