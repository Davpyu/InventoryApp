using DotNetService.Http.API.Version1.Role;

namespace DotNetService.Http.API.Version1.User
{
    public class UserResponse : Models.User
    {
        public List<RoleResponse> Roles { get; set; }

        public UserResponse(Models.User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Roles = RoleResponse.MapRepo(user.UserRoles?.Select(data => data.Role).ToList());
        }

        public static List<UserResponse> MapRepo(List<Models.User> data)
        {
            return data?.Select(data => new UserResponse(data)).ToList();
        }
    }
}