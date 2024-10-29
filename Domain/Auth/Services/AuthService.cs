using DotNetService.Http.API.Version1.Auth;
using DotNetService.Domain.User.Repositories;
using DotNetService.Domain.Role.Repositories;
using DotNetService.Domain.Permission.Repositories;
using DotNetService.Domain.RolePermission.Repositories;
using DotNetService.Domain.UserRole.Repositories;
using BC = BCrypt.Net.BCrypt;
using Newtonsoft.Json;
using DotNetService.Domain.Auth.Util;
using DotNetService.Infrastructure.Exceptions;

namespace DotNetService.Domain.Auth.Services
{
    public class AuthService(
        UserRoleQueryRepository userRoleQueryRepository,
        UserQueryRepository userQueryRepository,
        UserStoreRepository userStoreRepository,
        PermissionQueryRepository permissionQueryRepository,
        RoleQueryRepository roleQueryRepository,
        RolePermissionQueryRepository rolePermissionQueryRepository,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor
        )
    {
        private readonly UserRoleQueryRepository _userRoleQueryRepository = userRoleQueryRepository;
        private readonly UserQueryRepository _userQueryRepository = userQueryRepository;
        private readonly UserStoreRepository _userStoreRepository = userStoreRepository;
        private readonly PermissionQueryRepository _permissionQueryRepository = permissionQueryRepository;
        private readonly RoleQueryRepository _roleQueryRepository = roleQueryRepository;
        private readonly RolePermissionQueryRepository _rolePermissionQueryRepository = rolePermissionQueryRepository;
        private readonly IConfiguration _config = config;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<AuthInfo> SignIn(AuthSignInRequest authSignIn)
        {
            var user = await _userQueryRepository.FindOneByEmail(authSignIn.Email) ?? throw new UnauthenticatedException("Email or password is invalid");
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
            var isEmailExists = await _userQueryRepository.IsEmailExists(authRegister.Email);

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

            await _userStoreRepository.Create(data);
        }

        public async Task<Models.User> Account()
        {
            _ = Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("id")?.Value, out Guid userId);
            return await _userQueryRepository.FindOneById(userId);
        }

        public async Task<bool> IsUserHaveRole(Guid userId, string role)
        {
            var userRoles = await _userRoleQueryRepository.FindByUserId(userId);
            var roleIds = userRoles.Select(userRole => userRole.Id).ToArray();
            return await _roleQueryRepository.IsExistsByNameAndIds(role, roleIds);
        }

        public async Task<bool> IsRoleHavePermission(string role, string permission)
        {
            var roleRepository = await _roleQueryRepository.FindByName(role);
            var permissionRepository = await _permissionQueryRepository.FindByName(permission);
            Guid roleId = roleRepository.Id;
            Guid permissionId = permissionRepository.Id;

            var rolePermission = await _rolePermissionQueryRepository.FindByRoleAndPermission(roleId, permissionId);
            return rolePermission != null;
        }
    }
}