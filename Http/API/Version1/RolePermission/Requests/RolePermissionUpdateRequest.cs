using System.ComponentModel.DataAnnotations;

namespace DotNetService.Http.API.Version1.RolePermission
{
    public class RolePermissionUpdateRequest
    {
        [Required]
        public Guid Permissionid { get; set; }

        [Required]
        public Guid Roleid { get; set; }

    }
}