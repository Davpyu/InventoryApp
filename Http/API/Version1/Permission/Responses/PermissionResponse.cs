using DotNetService.Domain.Permission;
using System.Collections.Generic;

namespace DotNetService.Http.API.Version1.Permission
{
    public class PermissionResponse : Models.Permission
    {

        public PermissionResponse(Models.Permission permission)
        {
            this.Id = permission.Id;
            this.Name = permission.Name;
        }

        public static List<PermissionResponse> MapRepo(List<Models.Permission> data)
        {
            return data?.Select(data => new PermissionResponse(data)).ToList();
        }
    }
    
    public class PermissionItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public PermissionItem()
        {
        }

        public PermissionItem(Models.Permission permissionRepository)
        {
            this.Id = permissionRepository.Id;
            this.Name = permissionRepository.Name;
        }
        
        public static List<PermissionItem> MapRepo(List<Models.Permission> permissions)
        {
            var permissionMapped = new List<PermissionItem>();
            if (permissions == null)
            {
                return null;
            }

            foreach (Models.Permission permission in permissions)
            {
                permissionMapped.Add(new PermissionItem(permission));
            }

            return permissionMapped;
        }
    }
}