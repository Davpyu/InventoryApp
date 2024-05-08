using DotNetService.Domain.User;
using DotNetService.Http.API.Version1.Role;
using DotNetService.Http.API.Version1.UserRole;
using System.Collections.Generic;

namespace DotNetService.Http.API.Version1.User
{
    public class UserResponse : Models.User
    {
        public List<RoleResponse> Roles { get; set; }

        public UserResponse(Models.User user)
        {
            this.Id = user.Id;
            this.Name = user.Name;
            this.Email = user.Email;
            this.Roles = RoleResponse.MapRepo(user.UserRoles?.Select(data => data.Role).ToList());
        }

        public static List<UserResponse> MapRepo(List<Models.User> data)
        {
            return data?.Select(data => new UserResponse(data)).ToList();
        }
    }
}