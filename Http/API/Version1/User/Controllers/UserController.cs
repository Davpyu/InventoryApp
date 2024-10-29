using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Domain.User.Services;

namespace DotNetService.Http.API.Version1.User
{
    [Route("api/v1/users")]
    [ApiController]

    public class UserController(
        UserService userService
        ) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpGet()]
        public async Task<ApiResponse> Index([FromQuery] UserQueryRequest query)
        {
            return await _userService.Index(query);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> Show(Guid id)
        {
            Models.User data = await _userService.Detail(id);
            return new ApiResponseData(HttpStatusCode.OK, new UserResponse(data));
        }

        [HttpPost()]
        public async Task<ApiResponse> Store(UserCreateRequest dataCreate)
        {
            await _userService.Create(dataCreate);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> Update(Guid id, UserUpdateRequest dataUpdate)
        {
            await _userService.Update(id, dataUpdate);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _userService.Delete(id);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
