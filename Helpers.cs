using System.Net;
using DotNetService.Http.API.Version1.Responses;
using System.Runtime.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DotNetService
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
            IDictionary<string, string> validation = new Dictionary<string, string>();
            validation.Add("key", key);
            validation.Add("field", field);
            return validation;
        }
    }

    public class AuthUtility
    {
        public static string GenerateJwtToken(string secretKey, Guid id)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(GenerateSymetricKey(secretKey));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", id.ToString())
                }),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string GenerateSymetricKey(string key)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)).ToString();
        }


        public static Guid ValidateJwtTokenAndGetId(string secretKey, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(GenerateSymetricKey(secretKey));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                Guid id = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                return id;
            }
            catch
            {
                throw new UnauthorizedAccessException();
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