using DotNetService.Infrastructure.Shareds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using DotNetService.Constants.Logger;
using System.Net;
using DotNetService.Http.API.Version1.Responses;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

                var message = bool.Parse(_config["App:Debug"]) ? error.Message + " | " + error.StackTrace : error.Message;
                
                var statusCode = HttpStatusCode.InternalServerError;
                var validationError = ErrorValidation.ErrorModel(null);
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
                        statusCode = HttpStatusCode.InternalServerError;
                        break;
                }


                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var errorResponseValidation = new ApiResponseError(statusCode, message, validationError);
                await context.Response.WriteAsync(Utils.JsonSerialize(errorResponseValidation));
            }
        }
    }
}
