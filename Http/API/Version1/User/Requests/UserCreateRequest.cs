using System.ComponentModel.DataAnnotations;
using BC = BCrypt.Net.BCrypt;

namespace DotNetService.Http.API.Version1.User
{
    public class UserCreateRequest
    {
        [Required]
        [MinLength(6)]
        public string Name { get; set; }

        [Required]
        [MinLength(6)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

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