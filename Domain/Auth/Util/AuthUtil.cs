using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using DotNetService.Infrastructure.Exceptions;
using UserModel = DotNetService.Models.User;
using DotNetService.Models;

namespace DotNetService.Domain.Auth.Util
{
    public class AuthUtility
    {
        public const string LOCAL_STORAGE_KEY = "LOCAL_STORAGE_AUTH_KEY_";

        public static string GenerateJwtToken(string secretKey, string userJson, DateTime expires = default)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GenerateSymetricKey(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("user", userJson.ToString())
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string GenerateJWTClaimInfo(UserModel user, List<string> permissions = default) {
            return JsonSerializer.Serialize(new UserAuthInfo {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            });
        }
        
        public static ClaimsPrincipal ClaimPrincipalWithJson(dynamic user)
        {
            var userObject = (Dictionary<string, object>)JsonSerializer.Deserialize<Dictionary<string, object>>(user);

            var claims = userObject.Where(kvp => kvp.Key != null && kvp.Value != null)
                                   .Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()))
                                   .ToList();

            var identity = new ClaimsIdentity(claims, "User");

            return new ClaimsPrincipal(identity);
        }

        public static UserAuthInfo GenerateUserAuthInfo(UserModel user, List<string> permissions = default) {
            return new UserAuthInfo {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Permissions = permissions
            };
        }

        public static SymmetricSecurityKey GenerateSymetricKey(string key)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public static dynamic GetUserLogged(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDecoded = tokenHandler.ReadToken(token) as JwtSecurityToken;

                var userClaim = tokenDecoded.Claims.First(claim => claim.Type == "user");
                var userValue = userClaim.Value;

                var userObject = JsonSerializer.Deserialize<dynamic>(userValue);

                return userObject;
            }
            catch
            {
                throw new UnauthenticatedException();
            }
        }

        public static string GenerateKeyLocalStorage(string id) {
            return LOCAL_STORAGE_KEY + id;
        }

        private static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }

        public static JwtSecurityToken ValidateJwtToken(string secret, string tokenString)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secret));
                var handler = new JwtSecurityTokenHandler();
                var validation = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    LifetimeValidator = CustomLifetimeValidator,
                    RequireExpirationTime = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuerSigningKey = true,
                };
                var principal = handler.ValidateToken(tokenString, validation, out SecurityToken token);

                return (JwtSecurityToken)token;
            }
            catch
            {
                throw new UnauthenticatedException();
            }
        }
    }
}