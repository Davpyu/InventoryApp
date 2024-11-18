using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetService.Models
{
    public class UserRole : Base
    {
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public Guid RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}