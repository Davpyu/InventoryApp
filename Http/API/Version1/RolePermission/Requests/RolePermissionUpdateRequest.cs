using System.ComponentModel.DataAnnotations;

namespace DotNetService.Http.API.Version1.RolePermission
{
    public class RolePermissionUpdateRequest
    {
        [Required]
        public Guid PermissionId { get; set; }

        [Required]
        public Guid RoleId { get; set; }

    }
}