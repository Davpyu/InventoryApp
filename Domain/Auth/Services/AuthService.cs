using DotNetService.Http.API.Version1.Auth;
using BC = BCrypt.Net.BCrypt;
using Newtonsoft.Json;
using DotNetService.Domain.Auth.Util;
using DotNetService.Domain.Auth.Repositories;
using DotNetService.Infrastructure.Exceptions;

namespace DotNetService.Domain.Auth.Services
{
    public class AuthService(
        AuthStoreRepository authStoreRepository,
        AuthQueryRepository authQueryRepository,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor
        )
    {
        private readonly AuthStoreRepository _authStoreRepository = authStoreRepository;
        private readonly AuthQueryRepository _authQueryRepository = authQueryRepository;
        private readonly IConfiguration _config = config;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<AuthInfo> SignIn(AuthSignInRequest authSignIn)
        {
            var user = await authQueryRepository.FindOneByEmail(authSignIn.Email) ?? throw new UnauthenticatedException("Email or password is invalid");
            bool isPasswordVerified = BC.Verify(authSignIn.Password, user.Password);

            if (!isPasswordVerified)
            {
                throw new UnauthenticatedException("Email or password is invalid");
            }

            var tokenLifetimeInMinutes = int.Parse(_config["JWTSetting:LifetimeInMinutes"] ?? "60");
            var expiredAt = DateTime.Now.AddMinutes(tokenLifetimeInMinutes);

            user.Password = null;
            var userString = JsonConvert.SerializeObject(user);

            return new()
            {
                ExpiredAt = expiredAt,
                Token = AuthUtility.GenerateJwtToken(_config["JWTSetting:Secret"], userString, expiredAt)
            };
        }

        public async Task Register(AuthRegisterRequest authRegister)
        {
            var isEmailExists = await _authQueryRepository.IsEmailExist(authRegister.Email);

            if (isEmailExists)
            {
                throw new UnprocessableEntityException("Email already registered");
            }

            Models.User data = new()
            {
                Name = authRegister.Name,
                Email = authRegister.Email,
                Password = BC.HashPassword(authRegister.Password)
            };

            await _authStoreRepository.Create(data);
        }

        public async Task<Models.User> Account()
        {
            _ = Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("id")?.Value, out Guid userId);
            return await _authQueryRepository.FindOneById(userId);
        }
    }
}