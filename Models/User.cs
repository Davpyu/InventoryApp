using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetService.Models
{
    [Table("users")]
    public class User : Base
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("email")]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        public virtual List<UserRole> UserRoles { get; set; }
    }
}