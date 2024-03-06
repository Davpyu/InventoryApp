using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace DotNetService.Http.API.Version1.User
{
    public class UserQueryRequest : Query
    {
        [FromQuery(Name = "email")]
        public string Email { get; set; } // EXAMPLE: filter by email
    }
}