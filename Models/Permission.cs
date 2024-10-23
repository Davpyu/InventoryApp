using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetService.Models
{
    [Table("permissions")]
    public class Permission : Base
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("key")]
        [Index(IsUnique = true)]
        public string Key { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}