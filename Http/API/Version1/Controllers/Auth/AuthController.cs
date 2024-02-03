using Microsoft.AspNetCore.Mvc;
using DotNetService.Http.API.Version1.Requests.Auth;
using DotNetService.Http.API.Version1.Responses.Auth;
using System.Net;
using DotNetService.Domain.Auth.Services;
using DotNetService.Domain.User.Services;
using DotNetService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Authorization;

namespace DotNetService.Http.API.Version1.Controllers.Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController(
        AuthService authService,
        UserService userService
        ) : ControllerBase
    {
        private readonly AuthService _authService = authService;
        private readonly UserService _userService = userService;

        // GET: api/Book
        [AllowAnonymous]
        [HttpPost("sign-in")]
        [Consumes("application/json")]
        public ApiResponse SignIn(AuthSignIn authSignIn)
        {
            var authRepository = _authService.SignIn(authSignIn);
            var authToken = new AuthToken()
            {
                Token = authRepository.Token,
                ExpiredAt = authRepository.ExpiredAt
            };

            return new ApiResponseData(HttpStatusCode.OK, authToken);
        }

        [HttpPost("register")]
        [Consumes("application/json")]
        [AllowAnonymous]
        public ApiResponse Register(AuthRegister authRegister)
        {
            _userService.Register(authRegister);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
