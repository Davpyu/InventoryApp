using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetService.Http.API.Version1.Role.Requests
{
    public class RoleUpdateRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [JsonPropertyName("permission_ids")]
        public List<Guid> PermissionIds { get; set; }

        // [JsonPropertyName("permission_ids")]
        // public List<Guid> PermissionIds { get; set; }

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