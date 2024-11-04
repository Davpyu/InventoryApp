using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetService.Models
{
    [Table("user_role")]
    public class UserRole : Base
    {
        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Column("role_id")]
        public Guid RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}