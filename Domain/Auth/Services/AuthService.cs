using DotNetService.Http.API.Version1.Auth;
using BC = BCrypt.Net.BCrypt;
using DotNetService.Domain.Auth.Util;
using DotNetService.Domain.Auth.Repositories;
using DotNetService.Infrastructure.Exceptions;
using DotNetService.Domain.Permission.Repositories;
using DotNetService.Infrastructure.Databases;

namespace DotNetService.Domain.Auth.Services
{
    public class AuthService(
        AuthStoreRepository authStoreRepository,
        AuthQueryRepository authQueryRepository,
        PermissionQueryRepository permissionQueryRepository,
        IConfiguration config,
        LocalStorageDatabase localStorage,
        IHttpContextAccessor httpContextAccessor,
        AuthUtil authUtil
        )
    {
        private readonly AuthStoreRepository _authStoreRepository = authStoreRepository;
        private readonly AuthQueryRepository _authQueryRepository = authQueryRepository;
        private readonly PermissionQueryRepository _permissionQueryRepository = permissionQueryRepository;
        private readonly IConfiguration _config = config;
        private readonly LocalStorageDatabase _localStorage = localStorage; private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        private readonly AuthUtil _authUtil = authUtil;

        public async Task<AuthInfo> SignIn(AuthSignInRequest authSignIn)
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

            var permissions = await _permissionQueryRepository.FindPermissionByUserId(user.Id);

            var userObject = AuthUtil.GenerateUserAuthInfo(user, permissions);

            // store to local storage
            var localStorageKey = _authUtil.GenerateKeyLocalStorage(user.Id.ToString());
            await _localStorage.Store(localStorageKey, userObject);

            // encode user string
            var userString = AuthUtil.GenerateJWTClaimInfo(user);
            var token = AuthUtil.GenerateJwtToken(_config["JWTSetting:Secret"], userString, expiredAt);

            return new()
            {
                ExpiredAt = expiredAt,
                Token = token
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
            _ = Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value, out Guid userId);

            return await _authQueryRepository.FindOneById(userId);
        }
    }
}