using System.ComponentModel.DataAnnotations;
namespace DotNetService.Http.API.Version1.Permission
{
    public class PermissionCreateRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Key { get; set; }

        public static Models.Permission Assign(PermissionCreateRequest data)
        {
            Models.Permission res = new()
            {
                Name = data.Name,
                Key = data.Key
            };

            return res;
        }
    }
}