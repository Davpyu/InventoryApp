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

        [HttpGet()]
        public ApiResponse Index([FromQuery] Query query, [FromHeader] Header header)
        {
            if (query.Pagination)
            {
                List<Models.User> data = _userService.GetList(query.Search, query.Page, query.PerPage);
                int count = _userService.Count(query.Search);
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
                List<Models.User> data = _userService.GetList(query.Search, query.Page, query.PerPage);
                return new ApiResponseDataList(HttpStatusCode.OK, data, data.Count);
            }
        }

        [HttpGet("{id}")]
        public ApiResponse Show(Guid id)
        {
            Models.User data = _userService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new UserDetail(data));
        }

        [HttpPost()]
        [Consumes("application/json")]
        public ApiResponse Store(UserCreate userCreate)
        {
            _userService.Create(userCreate);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }

        [HttpPut("{id}")]
        public ApiResponse Update(Guid id, UserUpdate userUpdate)
        {
            _userService.Update(id, userUpdate);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        [HttpDelete("{id}")]
        public ApiResponse Delete(Guid id)
        {
            _userService.Delete(id);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
