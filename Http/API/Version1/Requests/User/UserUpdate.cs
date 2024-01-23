using System.ComponentModel.DataAnnotations;
namespace DotNetService.Http.API.Version1.Requests.User
{
    public class UserUpdate
    {
        [Required]
        [MinLength(6)]
        public string Name { get; set; }
    }
}