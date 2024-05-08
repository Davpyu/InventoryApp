using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace DotNetService.Http.API.Version1.Permission
{
    public class PermissionQueryRequest : Query
    {
        [FromQuery(Name = "name")]
        public string Name { get; set; }
    }
}