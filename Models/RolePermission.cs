using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DotNetService.Models
{
    [Table("role_permission")]
    public class RolePermission : Base
    {
        private readonly ILazyLoader _lazyLoader;

        [Column("role_id")]
        public Guid RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }

        [Column("permission_id")]
        public Guid PermissionId { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public virtual Permission Permission { get; set; }
    }
}