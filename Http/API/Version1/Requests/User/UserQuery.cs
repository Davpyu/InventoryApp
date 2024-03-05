using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace DotNetService.Http.API.Version1.Requests.User
{
    public class UserQuery : Query
    {
        [FromQuery(Name = "email")]
        public string Email { get; set; } // EXAMPLE: filter by email
    }
}