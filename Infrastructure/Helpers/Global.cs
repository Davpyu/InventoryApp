using System.Net;
using DotNetService.Http.API.Version1.Responses;
using System.Runtime.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetService.Exceptions;

namespace DotNetService.Infrastructure.Shareds
{
    public class ErrorUtility
    {
        public static List<IDictionary<string, string>> CreateSingleErrorValidation(string key, string field)
        {
            IDictionary<string, string> errorsValidation = ErrorUtility.SetErrorValidation(key, field);
            List<IDictionary<string, string>> validations = new List<IDictionary<string, string>>();
            validations.Add(errorsValidation);

            return validations;
        }
        public static IDictionary<string, string> SetErrorValidation(string key, string field)
        {
            IDictionary<string, string> validation = new Dictionary<string, string>
            {
                { "key", key },
                { "field", field }
            };
            return validation;
        }
    }

    public class AuthUtility
    {
        public static string GenerateJwtToken(string secretKey, Guid id, DateTime expires = default)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GenerateSymetricKey(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor() 
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", id.ToString())
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal ClaimPrincipalWithId(Guid id) {
            // Claim just id
            var identity = new ClaimsIdentity(
            [
                new ("id", id.ToString(), ClaimValueTypes.String)
            ], "User");

            return new ClaimsPrincipal(identity);
        }

        public static SymmetricSecurityKey GenerateSymetricKey(string key)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public static Guid GetId(string token)
        {
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDecoded = tokenHandler.ReadToken(token) as JwtSecurityToken;
                
                return new Guid(tokenDecoded.Claims.First(claim => claim.Type == "id")?.Value);
            } 
            catch {
                throw new UnauthenticatedException();
            }
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
            try {
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
            catch {
                throw new UnauthenticatedException();
            }
        }
    }
    
    [DataContract]
    public abstract class ApiResponse
    {
        [DataMember]
        public string Version { get { return "1.0.0"; } }
    }

    public class ApiResponseData(HttpStatusCode statusCode, object data = null) : ApiResponse
    {
        [DataMember]
        public int StatusCode { get; set; } = (int)statusCode;

        [DataMember(EmitDefaultValue = true)]
        public object Data { get; set; } = data;
    }

    public class ApiResponseDataList(HttpStatusCode statusCode, object items, int count) : ApiResponse
    {
        [DataMember]
        public int StatusCode { get; set; } = (int)statusCode;

        [DataMember(EmitDefaultValue = true)]
        public object Items { get; set; } = items;

        [DataMember(EmitDefaultValue = true)]
        public int Count { get; set; } = count;
    }

    public class ApiResponsePagination(HttpStatusCode statusCode, PaginationModel paginationModel) : ApiResponse
    {
        [DataMember]
        public int StatusCode { get; set; } = (int)statusCode;

        [DataMember(EmitDefaultValue = true)]
        public object items { get; set; } = paginationModel.Data;

        [DataMember(EmitDefaultValue = true)]
        public int Page { get; set; } = paginationModel.Page;

        [DataMember(EmitDefaultValue = true)]
        public int PerPage { get; set; } = paginationModel.PerPage;

        [DataMember(EmitDefaultValue = true)]
        public int Total { get; set; } = paginationModel.Total;

        [DataMember(EmitDefaultValue = true)]
        public int TotalPage { get; set; } = paginationModel.TotalPage;
    }

    public class ApiResponseError(HttpStatusCode statusCode, string errorMessage, object errors = null) : ApiResponse
    {
        [DataMember(EmitDefaultValue = true)]
        public string ErrorMessage { get; set; } = errorMessage;

        [DataMember(EmitDefaultValue = true)]
        public object Errors { get; set; } = errors;

        [DataMember]
        public int StatusCode { get; set; } = (int)statusCode;
    }
}