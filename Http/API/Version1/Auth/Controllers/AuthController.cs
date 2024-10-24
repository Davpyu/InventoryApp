using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetService.Domain.Auth.Services;
using DotNetService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Authorization;
using DotNetService.Http.API.Version1.User;

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
        public async Task<ApiResponse> SignIn(AuthSignInRequest authSignIn)
        {
            var authRepository = await _authService.SignIn(authSignIn);
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
        public async Task<ApiResponse> Register(AuthRegisterRequest authRegister)
        {
            await _authService.Register(authRegister);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        [HttpGet("account")]
        public async Task<ApiResponse> Account()
        {
            var data = await _authService.Account();
            return new ApiResponseData(HttpStatusCode.OK, new UserResponse(data));
        }
    }
}
