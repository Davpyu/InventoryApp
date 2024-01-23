using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DotNetService.Models
{
    [Table("role_permission")]
    public class RolePermission : Base
    {
        private readonly ILazyLoader _lazyLoader;

        [Column("id")]
        public Guid Id { get; set; }

        [Column("roleid")]
        public Guid Roleid { get; set; }

        [ForeignKey(nameof(Roleid))]
        public virtual Role Role { get; set; }

        [Column("permissionid")]
        public Guid Permissionid { get; set; }

        [ForeignKey(nameof(Permissionid))]
        public virtual Permission Permission { get; set; }
    }
}