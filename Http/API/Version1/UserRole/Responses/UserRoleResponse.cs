using DotNetService.Http.API.Version1.Role;
using DotNetService.Http.API.Version1.User;

namespace DotNetService.Http.API.Version1.UserRole
{
    public class UserRoleDetail
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public RoleItem Role { get; set; }
        public Guid UserId { get; set; }
        public UserResponse User { get; set; }

        public UserRoleDetail()
        {
        }

        public UserRoleDetail(Models.UserRole userRole)
        {
            Id = userRole.Id;
            RoleId = userRole.RoleId;
            UserId = userRole.UserId;
            User = new UserResponse(userRole.User);
            Role = new RoleItem(userRole.Role);
        }
    }

    public class UserRoleItem
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }

        public UserRoleItem()
        {
        }

        public UserRoleItem(Models.UserRole role)
        {
            Id = role.Id;
            RoleId = role.RoleId;
            UserId = role.UserId;
        }

        public static List<UserRoleItem> MapRepo(List<Models.UserRole> userRoles)
        {
            var userRolesMapped = new List<UserRoleItem>();
            if (userRoles == null)
            {
                return [];
            }

            foreach (Models.UserRole userRole in userRoles)
            {
                userRolesMapped.Add(new UserRoleItem(userRole));
            }

            return userRolesMapped;
        }
    }

    public class UserRoleList
    {
        public List<UserRoleDetail> Data { get; set; }
        public string Count { get; set; }
    }
}