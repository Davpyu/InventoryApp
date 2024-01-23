using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DotNetService.Models
{
    [Table("user_role")]
    public class UserRole : Base
    {

        [Column("id")]
        public Guid Id { get; set; }

        [Column("userid")]
        public Guid Userid { get; set; }

        [ForeignKey(nameof(Userid))]
        public virtual User User { get; set; }

        [Column("roleid")]
        public Guid Roleid { get; set; }

        [ForeignKey(nameof(Roleid))]
        public virtual Role Role { get; set; }
    }
}