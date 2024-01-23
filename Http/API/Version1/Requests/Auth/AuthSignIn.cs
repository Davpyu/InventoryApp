using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetService.Http.API.Version1.Requests.Auth
{
    public class AuthSignIn
    {
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}