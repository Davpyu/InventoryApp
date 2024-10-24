using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
namespace DotNetService.Http.API.Version1.Role
{
    public class RoleUpdateRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("permission_ids")]
        public List<Guid> PermissionIds { get; set; }

        public static Models.Role Assign(RoleUpdateRequest data)
        {
            Models.Role res = new()
            {
                Name = data.Name,
            };

            return res;
        }
    }
}