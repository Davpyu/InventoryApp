using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetService.Http.API.Version1.Role.Requests
{
    public class RoleCreateRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Key { get; set; }

        [Required]
        [JsonPropertyName("permission_ids")]
        public List<Guid> PermissionIds { get; set; }

        public static Models.Role Assign(RoleCreateRequest data)
        {
            Models.Role res = new()
            {
                Name = data.Name,
                Key = data.Key
            };

            return res;
        }
    }
}