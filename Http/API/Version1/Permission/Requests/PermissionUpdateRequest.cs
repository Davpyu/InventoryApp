using System.ComponentModel.DataAnnotations;
namespace DotNetService.Http.API.Version1.Permission
{
    public class PermissionUpdateRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        public static Models.Permission Assign(PermissionUpdateRequest data)
        {
            Models.Permission res = new()
            {
                Name = data.Name,

            };

            return res;
        }
    }
}