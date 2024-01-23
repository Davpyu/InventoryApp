using System;

namespace DotNetService.Domain.Auth
{
    public class AuthInfo
    {
        public string Token { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}