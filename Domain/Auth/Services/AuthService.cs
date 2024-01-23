using DotNetService.Http.API.Version1.Requests.Auth;
using DotNetService.Domain.User.Repositories;
using DotNetService.Domain.Role.Repositories;
using DotNetService.Domain.Permission.Repositories;
using DotNetService.Domain.RolePermission.Repositories;
using DotNetService.Domain.UserRole.Repositories;
using DotNetService.Exceptions;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Mvc;
using DotNetService.Infrastructure.Events;

namespace DotNetService.Domain.Auth.Services
{
    public class AuthService(
        UserRoleQueryRepository userRoleQueryRepository,
        UserQueryRepository userQueryRepository,
        PermissionQueryRepository permissionQueryRepository,
        RoleQueryRepository roleQueryRepository,
        RolePermissionQueryRepository rolePermissionQueryRepository,
        IConfiguration config
        )
    {
        private readonly UserRoleQueryRepository _userRoleQueryRepository = userRoleQueryRepository;
        private readonly UserQueryRepository _userQueryRepository = userQueryRepository;
        private readonly PermissionQueryRepository _permissionQueryRepository = permissionQueryRepository;
        private readonly RoleQueryRepository _roleQueryRepository = roleQueryRepository;
        private readonly RolePermissionQueryRepository _rolePermissionQueryRepository = rolePermissionQueryRepository;
        private readonly IConfiguration _config = config;

        public AuthInfo SignIn(AuthSignIn authSignIn)
        {
            var userRepo = _userQueryRepository.FindByEmail(authSignIn.Email);
            if (userRepo == null || userRepo.Email == null || !BC.Verify(authSignIn.Password, userRepo.Password))
            {
                throw new DataNotFoundException();
            }
            
            var expiredAt = DateTime.UtcNow.AddYears(1);
            return new ()
            {
                ExpiredAt = expiredAt,
                Token = AuthUtility.GenerateJwtToken(_config["App:DataProtectionKey"], userRepo.Id)
            };
        }

        public bool IsUserHaveRole(Guid userId, string role)
        {
            var userRoles = _userRoleQueryRepository.FindByUserId(userId);
            var roleIds = userRoles.Select(userRole => userRole.Id).ToArray();
            return _roleQueryRepository.IsExistsByNameAndIds(role, roleIds);
        }

        public bool IsRoleHavePermission(string role, string permission)
        {
            var roleRepository = _roleQueryRepository.FindByName(role);
            var permissionRepository = _permissionQueryRepository.FindByName(permission);
            Guid roleId = roleRepository.Id;
            Guid permissionId = permissionRepository.Id;

            var rolePermission = _rolePermissionQueryRepository.FindByRoleAndPermission(roleId, permissionId);
            return rolePermission != null;
        }
    }
}