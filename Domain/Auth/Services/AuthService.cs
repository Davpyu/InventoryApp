using DotNetService.Http.API.Version1.Auth;
using BC = BCrypt.Net.BCrypt;
using Newtonsoft.Json;
using DotNetService.Domain.Auth.Util;
using DotNetService.Domain.Auth.Repositories;
using DotNetService.Infrastructure.Exceptions;
using DotNetService.Domain.Permission.Repositories;
using DotNetService.Domain.Auth.Dtos;

namespace DotNetService.Domain.Auth.Services
{
    public class AuthService(
        AuthStoreRepository authStoreRepository,
        AuthQueryRepository authQueryRepository,
        PermissionQueryRepository permissionQueryRepository,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor
        )
    {
        private readonly AuthStoreRepository _authStoreRepository = authStoreRepository;
        private readonly AuthQueryRepository _authQueryRepository = authQueryRepository;
        private readonly PermissionQueryRepository _permissionQueryRepository = permissionQueryRepository;
        private readonly IConfiguration _config = config;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<AuthTokenResultDto> SignIn(AuthSignInDto authSignIn)
        {
            var user = await _authQueryRepository.FindOneByEmail(authSignIn.Email);
            if (user == null)
            {
                throw new UnauthenticatedException("Email or password is incorrect.");
            }

            bool isPasswordVerified = BC.Verify(authSignIn.Password, user.Password);

            if (!isPasswordVerified)
            {
                throw new UnauthenticatedException("Email or password is incorrect.");
            }

            var tokenLifetimeInMinutes = int.Parse(_config["JWTSetting:LifetimeInMinutes"] ?? "60");
            var expiredAt = DateTime.Now.AddMinutes(tokenLifetimeInMinutes);

            user.Password = null;
            var permissions = await _permissionQueryRepository.FindPermissionByUserId(user.Id);
            var userString = JsonConvert.SerializeObject(new
            {
                user.Id,
                user.Name,
                user.Email,
                permissions
            });

            return new AuthTokenResultDto
            {
                ExpiredAt = expiredAt,
                Token = AuthUtility.GenerateJwtToken(_config["JWTSetting:Secret"], userString, expiredAt),
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

        public async Task<AccountResultDto> Account()
        {
            _ = Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("id")?.Value, out Guid userId);

            var user = await _authQueryRepository.FindOneById(userId);
            if (user == null)
            {
                throw new DataNotFoundException("User not found");
            }

            return new AccountResultDto(user);
        }
    }
}