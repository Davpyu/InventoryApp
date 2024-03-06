using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetService.Http.API.Version1.Auth
{
    public class AuthSignInRequest
    {
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}