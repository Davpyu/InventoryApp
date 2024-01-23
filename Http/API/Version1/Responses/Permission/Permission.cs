using DotNetService.Domain.Permission;
using System.Collections.Generic;

namespace DotNetService.Http.API.Version1.Responses.Permission
{
    public class PermissionDetail
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public PermissionDetail()
        {
        }

        public PermissionDetail(Models.Permission permissionRepository)
        {
            this.Id = permissionRepository.Id;
            this.Name = permissionRepository.Name;
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

    public class PermissionList
    {

        public List<PermissionDetail> data { get; set; }
        public string count { get; set; }
    }
}