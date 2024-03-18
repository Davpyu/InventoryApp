using DotNetService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetService.Http
{
    [Route("health")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController
    {
        [HttpGet]
        public ApiResponseData Get()
        {
            return new ApiResponseData(System.Net.HttpStatusCode.OK, new { message = "Service is running" });
        }
    }
}
