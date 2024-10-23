using System.ComponentModel.DataAnnotations;
using DotNetService.Infrastructure.Regexs;

namespace DotNetService.Http.API.Version1.Auth
{
    public class AuthRegisterRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MinLength(5)]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(30)]
        [RegularExpression(AuthRegex.PASSWORD)]
        public string Password { get; set; }
    }
}