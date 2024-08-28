using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Domain.User.Services;
using Microsoft.AspNetCore.Authorization;

namespace DotNetService.Http.API.Version1.User
{
    [Route("api/v1/users")]
    [ApiController]
    [AllowAnonymous]
    public class UserController(
        UserService userService
        ) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpGet()]
        public ApiResponse Index([FromQuery] UserQueryRequest query)
        {
            return _userService.Index(query);
        }

        [HttpGet("{id}")]
        public ApiResponse Show(Guid id)
        {
            Models.User data = _userService.Detail(id);
            data = null;
            return new ApiResponseData(HttpStatusCode.OK, new UserResponse(data));
        }

        [HttpPost()]
        public ApiResponse Store(UserCreateRequest dataCreate)
        {
            var data = _userService.Create(dataCreate);
            return new ApiResponseData(HttpStatusCode.OK, new UserResponse(data));
        }

        [HttpPut("{id}")]
        public ApiResponse Update(Guid id, UserUpdateRequest dataUpdate)
        {
            var data = _userService.Update(id, dataUpdate);
            return new ApiResponseData(HttpStatusCode.OK, new UserResponse(data));
        }

        [HttpDelete("{id}")]
        public ApiResponse Delete(Guid id)
        {
            _userService.Delete(id);
            return new ApiResponseData(HttpStatusCode.OK, new UserResponse(null));
        }
    }
}
