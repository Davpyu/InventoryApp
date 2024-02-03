using Microsoft.AspNetCore.Mvc;
using DotNetService.Http.API.Version1.Requests;
using DotNetService.Http.API.Version1.Requests.User;
using DotNetService.Http.API.Version1.Responses;
using DotNetService.Http.API.Version1.Responses.User;
using System.Net;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Domain.User.Services;

namespace DotNetService.Http.API.Version1.Controllers.IAM
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(
            UserService userService
        )
        {
            _userService = userService;
        }

        // GET: api/User
        [HttpGet()]
        public ApiResponse Index([FromQuery] Query query, [FromHeader] Header header)
        {
            if (query.Pagination)
            {
                var usersRepo = _userService.GetList(query.Search, query.Page, query.PerPage);
                int count = _userService.Count(query.Search);
                decimal pageInCount = ((decimal)count) / query.PerPage;
                var paginate = new PaginationModel()
                {
                    TotalPage = (int)Math.Ceiling(pageInCount),
                    Page = query.Page,
                    PerPage = query.PerPage,
                    Data = UserItem.MapRepo(usersRepo),
                    Total = count
                };

                return new ApiResponsePagination(HttpStatusCode.OK, paginate);
            }
            else
            {
                var usersRepo = _userService.GetList(query.Search, query.Page, query.PerPage);
                return new ApiResponseDataList(HttpStatusCode.OK, usersRepo, usersRepo.Count);
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public ApiResponse Show(Guid id)
        {
            var user = _userService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new UserDetail(user));
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public ApiResponse Update(Guid id, UserUpdate userUpdate)
        {
            _userService.Update(id, userUpdate);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ApiResponse Delete(Guid id)
        {
            _userService.Delete(id);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
