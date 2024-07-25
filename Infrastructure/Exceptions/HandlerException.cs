using DotNetService.Infrastructure.Shareds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using DotNetService.Constants.Logger;
using System.Net;
using DotNetService.Http.API.Version1;
using System.Net.Mime;

namespace DotNetService.Exceptions
{
    public class HandlerException(
        RequestDelegate next,
        IConfiguration config,
        ILoggerFactory loggerFactory
    )
    {
        private readonly RequestDelegate _next = next;
        private readonly IConfiguration _config = config;
        private readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.ERROR);


        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var stackTrace = bool.Parse(_config["App:Debug"]) ? error.StackTrace?.Trim() : null;
                string errorMessage = error.Message;
                var validationError = ErrorValidation.ErrorModel(null);

                _logger.LogError(error, "{errorMessage}", errorMessage);

                HttpStatusCode statusCode;
                switch (error)
                {
                    // case when request validation failure
                    case ValidationException e:
                        statusCode = HttpStatusCode.BadRequest;
                        validationError = ErrorValidation.ErrorModel(e.ModelState);
                        break;
                    // case when request validation failure
                    case BusinessException e:
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    case DataNotFoundException e:
                        statusCode = HttpStatusCode.NotFound;
                        break;
                    case BadHttpRequestException e:
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    case UnprocessableEntityException e:
                        statusCode = HttpStatusCode.UnprocessableEntity;
                        break;
                    case DbUpdateException e:
                        statusCode = HttpStatusCode.BadRequest;
                        if (e.InnerException is SqlException sqlException)
                        {
                            switch (sqlException.Number)
                            {
                                case 2601:
                                    break;
                            }
                        }
                        break;
                    // case for unhandled exception
                    case UnauthenticatedException e:
                        statusCode = HttpStatusCode.Unauthorized;
                        break;
                    case UnauthorizedAccessException e:
                        statusCode = HttpStatusCode.Unauthorized;
                        break;
                    case ServiceUnavailableException e:
                        statusCode = HttpStatusCode.InternalServerError;
                        break;
                    // case for unhandled exception
                    case NotAllowedException e:
                        statusCode = HttpStatusCode.Forbidden;
                        break;
                    // case for unhandled exception
                    default:
                        // log error to sentry 500 error only
                        SentrySdk.CaptureException(error);
                        statusCode = HttpStatusCode.InternalServerError;
                        var errorFormat = new Dictionary<string, string>
                        {
                            {"Type", error.GetType().ToString()},
                            {"Message", error.Message},
                            {"Source", error.Source}
                        };
                        if (config["App:Environment"] == "Development")
                        {
                            errorFormat.Add("StackTrace", error.StackTrace);
                        }
                        break;
                }


                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var errorResponseValidation = new ApiResponseError(statusCode, errorMessage, validationError, stackTrace);
                await context.Response.WriteAsync(Utils.JsonSerialize(errorResponseValidation));
            }
        }
    }
}
