using System;

namespace DotNetService.Http.API.Version1.Responses.Auth
{
    public class AuthToken
    {
        public DateTime ExpiredAt { get; set; }

        public string Token { get; set; }
    }
}