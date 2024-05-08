using DotNetService.Domain.Role;
using DotNetService.Http.API.Version1.Permission;
using DotNetService.Http.API.Version1.UserRole;
using System.Collections.Generic;

namespace DotNetService.Http.API.Version1.Role
{
    public class RoleResponse : Models.Role
    {
        public new List<UserRoleItem> UserRoles { get; set; }
        public List<PermissionResponse> Permissions { get; set; }

        public RoleResponse(Models.Role role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
            this.Permissions = role.RolePermissions?.Count > 0 ? PermissionResponse.MapRepo(role.RolePermissions?.Select(data => data.Permission).ToList()) : null;
        }

        public static List<RoleResponse> MapRepo(List<Models.Role> data)
        {
            return data?.Select(data => new RoleResponse(data)).ToList();
        }
    }

    public class RoleItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public RoleItem()
        {
        }

        public RoleItem(Models.Role roleRepository)
        {
            this.Id = roleRepository.Id;
            this.Name = roleRepository.Name;
        }
        public static List<RoleItem> MapRepo(List<Models.Role> roles)
        {
            var roleMapped = new List<RoleItem>();
            if (roles == null)
            {
                return null;
            }

            foreach (Models.Role role in roles)
            {
                roleMapped.Add(new RoleItem(role));
            }

            return roleMapped;
        }
    }
}