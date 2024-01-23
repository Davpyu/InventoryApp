using System.ComponentModel.DataAnnotations;
namespace DotNetService.Http.API.Version1.Requests.Role
{
    public class RoleCreate
    {
        [Required]
        [MinLength(6)]
        public string Name { get; set; }
    }
}