using DotNetService.Http.API.Version1.User;
using DotNetService.Domain.User.Repositories;
using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1;
using DotNetService.Http.API.Version1.User;
using DotNetService.Infrastructure.Shareds;
using System.Net;

namespace DotNetService.Domain.User.Services
{
    public class UserService(
            UserQueryRepository userQueryRepository,
            UserStoreRepository userStoreRepository
        )
    {
        private readonly UserQueryRepository _userQueryRepository = userQueryRepository;
        private readonly UserStoreRepository _userStoreRepository = userStoreRepository;

        public ApiResponse Index(UserQueryRequest query = null)
        {
            if (query.Pagination)
            {
                List<Models.User> data = Pagination(query);
                int count = Count(query.Search);
                decimal pageInCount = ((decimal)count) / query.PerPage;
                PaginationModel paginate = new()
                {
                    TotalPage = (int)Math.Ceiling(pageInCount),
                    Page = query.Page,
                    PerPage = query.PerPage,
                    Data = UserItem.MapRepo(data),
                    Total = count
                };

                return new ApiResponsePagination(HttpStatusCode.OK, paginate);
            }
            else
            {
                List<Models.User> data = Pagination(query);
                return new ApiResponseDataList(HttpStatusCode.OK, data, data.Count);
            }
        }

        public List<Models.User> Pagination(UserQueryRequest query = null)
        {
            return _userQueryRepository.Pagination(query);
        }

        public void Create(UserCreateRequest userCreate)
        {
            Models.User data = new()
            {
                Name = userCreate.Name
            };

            _userStoreRepository.Create(data);
        }

        public void Update(Guid id, UserUpdateRequest userUpdate)
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

        public int Count(string search)
        {
            return _userQueryRepository.CountAll(search);
        }
    }
}