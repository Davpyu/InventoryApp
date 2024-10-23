using System.ComponentModel.DataAnnotations;
using DotNetService.Infrastructure.Regexs;

namespace DotNetService.Http.API.Version1.Auth
{
    public class AuthSignInRequest
    {
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