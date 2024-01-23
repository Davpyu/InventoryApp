using System.ComponentModel.DataAnnotations;

namespace DotNetService.Http.API.Version1.Requests.RolePermission
{
    public class RolePermissionCreate
    {
        [Required]
        public Guid Permissionid { get; set; }

        [Required]
        public Guid Roleid { get; set; }
    }

}