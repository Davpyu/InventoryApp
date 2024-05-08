using System.ComponentModel.DataAnnotations;
namespace DotNetService.Http.API.Version1.Role
{
    public class RoleUpdateRequest
    {
        [Required]
        public string Name { get; set; }

        public List<Guid> PermissionIds { get; set; }
    }
}