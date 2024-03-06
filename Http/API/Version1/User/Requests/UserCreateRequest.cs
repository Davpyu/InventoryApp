using System.ComponentModel.DataAnnotations;
namespace DotNetService.Http.API.Version1.User
{
    public class UserCreateRequest
    {
        [Required]
        [MinLength(6)]
        public string Name { get; set; }
    }
}