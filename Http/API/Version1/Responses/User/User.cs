using DotNetService.Domain.User;
using DotNetService.Http.API.Version1.Responses.UserRole;
using System.Collections.Generic;

namespace DotNetService.Http.API.Version1.Responses.User
{
    public class UserDetail
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public UserDetail()
        {
        }

        public UserDetail(Models.User user)
        {
            this.Id = user.Id;
            this.Name = user.Name;
            this.Email = user.Email;
        }
    }
    public class UserItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UserRoleItem> UserRoles { get; set; }
        public string Email { get; set; }

        public UserItem()
        {
        }

        public UserItem(Models.User user)
        {
            this.Id = user.Id;
            this.Name = user.Name;
            this.Email = user.Email;
            this.UserRoles = UserRoleItem.MapRepo(user.UserRoles?.ToList());
        }
        public static List<UserItem> MapRepo(List<Models.User> userRepositories)
        {
            var Books = new List<UserItem>();
            if (userRepositories == null)
            {
                return [];
            }

            foreach (Models.User user in userRepositories)
            {
                Books.Add(new UserItem(user));
            }

            return Books;
        }
    }
    public class UserList
    {

        public List<UserDetail> Data { get; set; }
        public string Count { get; set; }
    }
}