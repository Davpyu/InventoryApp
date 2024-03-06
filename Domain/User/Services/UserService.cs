using DotNetService.Http.API.Version1.User;
using DotNetService.Domain.User.Repositories;
using DotNetService.Http.API.Version1;
using DotNetService.Infrastructure.Shareds;
using System.Net;
using BC = BCrypt.Net.BCrypt;

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
                var data = Pagination(query);
                int count = Count(query.Search);
                decimal pageInCount = ((decimal)count) / query.PerPage;
                PaginationModel paginate = new()
                {
                    TotalPage = (int)Math.Ceiling(pageInCount),
                    Page = query.Page,
                    PerPage = query.PerPage,
                    Data = UserResponse.MapRepo(data),
                    Total = count
                };

                return new ApiResponsePagination(HttpStatusCode.OK, paginate);
            }
            else
            {
                var data = Pagination(query);
                return new ApiResponseDataList(HttpStatusCode.OK, data, data.Count);
            }
        }

        public List<Models.User> Pagination(UserQueryRequest query = null)
        {
            return _userQueryRepository.Pagination(query);
        }

        public Models.User Create(UserCreateRequest dataCreate)
        {
            Models.User data = new()
            {
                Name = dataCreate.Name,
                Email = dataCreate.Email,
                Password = BC.HashPassword(dataCreate.Password)

            };

            return _userStoreRepository.Create(data);
        }

        public Models.User Update(Guid id, UserUpdateRequest dataUpdate)
        {
            Models.User data = new()
            {
                Name = dataUpdate.Name,
                Email = dataUpdate.Email,
                Password = BC.HashPassword(dataUpdate.Password)
            };

            return _userStoreRepository.Update(id, data);
        }

        public Models.User Delete(Guid id)
        {
            return _userStoreRepository.Delete(id);
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