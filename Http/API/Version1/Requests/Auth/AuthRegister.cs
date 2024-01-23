using System.ComponentModel.DataAnnotations;

namespace DotNetService.Http.API.Version1.Requests.Auth
{
    public class AuthRegister
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}