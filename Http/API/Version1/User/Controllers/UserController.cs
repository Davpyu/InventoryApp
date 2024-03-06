using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Domain.User.Services;

namespace DotNetService.Http.API.Version1.User
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
        public ApiResponse Index([FromQuery] UserQueryRequest query, [FromHeader] Header header)
        {
            return _userService.Index(query);
        }

        [HttpGet("{id}")]
        public ApiResponse Show(Guid id)
        {
            Models.User data = _userService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new UserDetail(data));
        }

        [HttpPost()]
        [Consumes("application/json")]
        public ApiResponse Store(UserCreateRequest userCreate)
        {
            _userService.Create(userCreate);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }

        [HttpPut("{id}")]
        public ApiResponse Update(Guid id, UserUpdateRequest userUpdate)
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
