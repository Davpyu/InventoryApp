using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace DotNetService.Http.API.Version1.Role
{
    public class RoleQueryRequest : Query
    {
        [FromQuery(Name = "name")]
        public string Name { get; set; }
    }
}