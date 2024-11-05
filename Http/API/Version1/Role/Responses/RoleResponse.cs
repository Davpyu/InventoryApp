using DotNetService.Http.API.Version1.Permission;

namespace DotNetService.Http.API.Version1.Role
{
    public class RoleResponse : Models.Role
    {
        public List<PermissionResponse> Permissions { get; set; }

        public RoleResponse(Models.Role role)
        {
            Id = role.Id;
            Name = role.Name;
            Key = role.Key;
            Permissions = role.RolePermissions?.Count > 0 ? PermissionResponse.MapRepo(role.RolePermissions?.Select(data => data.Permission).ToList()) : null;
        }

        public static List<RoleResponse> MapRepo(List<Models.Role> data)
        {
            return data?.Select(data => new RoleResponse(data)).ToList();
        }
    }

    public class RoleItem(Models.Role roleRepository)
    {
        public Guid Id { get; set; } = roleRepository.Id;
        public string Name { get; set; } = roleRepository.Name;

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