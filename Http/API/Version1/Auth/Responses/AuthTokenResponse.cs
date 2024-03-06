using System;

namespace DotNetService.Http.API.Version1.Auth
{
    public class AuthTokenResponse
    {
        public DateTime ExpiredAt { get; set; }

        public string Token { get; set; }
    }
}