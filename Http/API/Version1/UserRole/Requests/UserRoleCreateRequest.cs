using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DotNetService.Http.API.Version1.UserRole
{
    public class UserRoleCreateRequest
    {
        [Required]
        public Guid Userid { get; set; }
        [Required]
        public Guid Roleid { get; set; }
    }

}