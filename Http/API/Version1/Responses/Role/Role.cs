using DotNetService.Domain.Role;
using System.Collections.Generic;

namespace DotNetService.Http.API.Version1.Responses.Role
{
    public class RoleDetail
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public RoleDetail()
        {
        }

        public RoleDetail(Models.Role roleRepository)
        {
            this.Id = roleRepository.Id;
            this.Name = roleRepository.Name;
        }
    }
    public class RoleItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public RoleItem()
        {
        }

        public RoleItem(Models.Role roleRepository)
        {
            this.Id = roleRepository.Id;
            this.Name = roleRepository.Name;
        }
        public static List<RoleItem> MapRepo(List<Models.Role> roles)
        {
            var roleMapped = new List<RoleItem>();
            if (roles == null)
            {
                return null;
            }

            foreach (Models.Role role in roles)
            {
                roleMapped.Add(new RoleItem(role));
            }

            return roleMapped;
        }
    }
    public class RoleList
    {

        public List<RoleDetail> data { get; set; }
        public string count { get; set; }
    }
}