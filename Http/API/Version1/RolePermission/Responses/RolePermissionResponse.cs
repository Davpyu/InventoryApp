using DotNetService.Domain.RolePermission;
using DotNetService.Http.API.Version1.Role;
using DotNetService.Http.API.Version1.Permission;
using System.Collections.Generic;

namespace DotNetService.Http.API.Version1.RolePermission
{
    public class RolePermissionDetail
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public RoleItem Role { get; set; }
        public Guid PermissionId { get; set; }
        public PermissionItem Permission { get; set; }

        public RolePermissionDetail()
        {
        }

        public RolePermissionDetail(Models.RolePermission rolePermissionRepository)
        {
            this.Id = rolePermissionRepository.Id;
            this.RoleId = rolePermissionRepository.RoleId;
            this.PermissionId = rolePermissionRepository.PermissionId;
            this.Permission = (new PermissionItem(rolePermissionRepository.Permission));
            this.Role = (new RoleItem(rolePermissionRepository.Role));
        }
    }
    public class RolePermissionItem
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public RolePermissionItem()
        {
        }

        public RolePermissionItem(Models.RolePermission rolePermissionRepository)
        {
            this.Id = rolePermissionRepository.Id;
            this.RoleId = rolePermissionRepository.RoleId;
            this.PermissionId = rolePermissionRepository.PermissionId;
        }

        public static List<RolePermissionItem> MapRepo(List<Models.RolePermission> rolePermissions)
        {
            var rolePermissionsMapped = new List<RolePermissionItem>();
            if (rolePermissions == null)
            {
                return null;
            }

            foreach (Models.RolePermission rolePermission in rolePermissions)
            {
                rolePermissionsMapped.Add(new RolePermissionItem(rolePermission));
            }

            return rolePermissionsMapped;
        }
    }

    public class RolePermissionList
    {
        public List<RolePermissionDetail> Data { get; set; }
        public string Count { get; set; }
    }
}