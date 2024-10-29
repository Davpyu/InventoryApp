using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DotNetService.Infrastructure.Regexs;
using BC = BCrypt.Net.BCrypt;

namespace DotNetService.Http.API.Version1.User
{
    public class UserCreateRequest
    {
        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(30)]
        [RegularExpression(AuthRegex.PASSWORD, ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]

        public string Password { get; set; }

        [Required]
        [JsonPropertyName("role_ids")]
        public List<Guid> RoleIds { get; set; }

        public static Models.User Assign(UserCreateRequest data)
        {
            Models.User res = new()
            {
                Name = data.Name,
                Email = data.Email,
                Password = BC.HashPassword(data.Password)
            };

            return res;
        }
    }
}