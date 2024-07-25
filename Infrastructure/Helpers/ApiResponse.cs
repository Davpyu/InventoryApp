using System.Net;
using DotNetService.Http.API.Version1;
using System.Runtime.Serialization;

namespace DotNetService.Infrastructure.Shareds
{
    public class ErrorUtility
    {
        public static List<IDictionary<string, string>> CreateSingleErrorValidation(string key, string field)
        {
            IDictionary<string, string> errorsValidation = ErrorUtility.SetErrorValidation(key, field);
            List<IDictionary<string, string>> validations = [errorsValidation];

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
        public object Items { get; set; } = paginationModel.Data;

        [DataMember(EmitDefaultValue = true)]
        public PaginationMeta Meta { get; set; } = new()
        {
            TotalPage = paginationModel.TotalPage,
            Total = paginationModel.Total,
            Page = paginationModel.Page,
            PerPage = paginationModel.PerPage
        };
    }

    public class ApiResponseError(HttpStatusCode statusCode, string errorMessage, object errors = null, string stackTrace = null) : ApiResponse
    {
        [DataMember(EmitDefaultValue = true)]
        public string ErrorMessage { get; set; } = errorMessage;

        [DataMember(EmitDefaultValue = true)]
        public string StackTrace { get; set; } = stackTrace;

        [DataMember(EmitDefaultValue = true)]
        public object Errors { get; set; } = errors;

        [DataMember]
        public int StatusCode { get; set; } = (int)statusCode;
    }
}