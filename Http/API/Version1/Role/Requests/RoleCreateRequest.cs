using System.ComponentModel.DataAnnotations;
namespace DotNetService.Http.API.Version1.Role
{
    public class RoleCreateRequest
    {
        [Required]
        [MinLength(6)]
        public string Name { get; set; }
    }
}