using System.ComponentModel.DataAnnotations;
namespace DotNetService.Http.API.Version1.Role
{
    public class RoleUpdateRequest
    {
        [Required]
        public string Name { get; set; }

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