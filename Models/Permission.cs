using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetService.Models
{
    public class Permission : Base
    {
        public string Name { get; set; }

        [Index(IsUnique = true)]
        public string Key { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}