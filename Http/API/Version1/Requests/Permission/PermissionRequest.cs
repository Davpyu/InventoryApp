using System.ComponentModel.DataAnnotations;
namespace DotNetService.Http.API.Version1.Requests.Permission
{
    public class PermissionCreate
    {
        [Required]
        [MinLength(6)]
        public string Name { get; set; }
    }
    public class PermissionUpdate
    {
        [Required]
        [MinLength(6)]
        public string Name { get; set; }
    }
}