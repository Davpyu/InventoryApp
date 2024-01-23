using Microsoft.AspNetCore.Mvc;

namespace DotNetService.Http.API.Version1.Requests
{
    public class Header
    {
        [FromHeader(Name = "Authorization")]
        public string Authorization { get; set; }
        
        [FromHeader(Name = "Version")]
        public string Version { get; set; }

        [FromHeader(Name = "Locale")]
        public string Locale { get; set; }

        [FromHeader(Name = "Platform")]
        public string Platform { get; set; }
    }
}