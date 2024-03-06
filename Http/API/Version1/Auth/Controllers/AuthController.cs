using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetService.Domain.Auth.Services;
using DotNetService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Authorization;

namespace DotNetService.Http.API.Version1.Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController(
        AuthService authService
        ) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        // GET: api/Book
        [AllowAnonymous]
        [HttpPost("sign-in")]
        [Consumes("application/json")]
        public ApiResponse SignIn(AuthSignInRequest authSignIn)
        {
            var authRepository = _authService.SignIn(authSignIn);
            var authToken = new AuthTokenResponse()
            {
                Token = authRepository.Token,
                ExpiredAt = authRepository.ExpiredAt
            };

            return new ApiResponseData(HttpStatusCode.OK, authToken);
        }

        [HttpPost("register")]
        [Consumes("application/json")]
        [AllowAnonymous]
        public ApiResponse Register(AuthRegisterRequest authRegister)
        {
            _authService.Register(authRegister);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
