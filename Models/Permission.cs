using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetService.Models
{
    [Table("permissions")]
    public class Permission : Base
    {
        [Column("name")]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}