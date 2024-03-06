using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DotNetService.Http.API.Version1.UserRole
{
    public class UserRoleUpdateRequest
    {
        [Required]
        public Guid Userid { get; set; }
        [Required]
        public Guid Roleid { get; set; }
    }
}