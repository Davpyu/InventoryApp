using DotNetService.Domain.Role.Dtos;

namespace DotNetService.Domain.User.Dtos
{
    public class UserResultDto : Models.User
    {
        public List<RoleResultDto> Roles { get; set; }

        public UserResultDto(Models.User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Roles = RoleResultDto.MapRepo(user.UserRoles?.Select(data => data.Role).ToList());
        }

        public static List<UserResultDto> MapRepo(List<Models.User> data)
        {
            return data?.Select(data => new UserResultDto(data)).ToList();
        }
    }

    public class UserResultLiteDto(Models.User user)
    {
        public Guid Id { get; set; } = user.Id;
        public string Name { get; set; } = user.Name;
        public string Email { get; set; } = user.Email;

        public List<RoleItem> Roles { get; set; } = RoleItem.MapRepo(user.UserRoles?.Select(data => data.Role).ToList());
    }
}