using DotNetService.Exceptions;
using Sentry;

namespace DotNetService.Infrastructure.Middlewares {
    public class SentryMiddleware
    {
        private readonly RequestDelegate _next;


        public SentryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Panggil middleware selanjutnya dalam pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                //filter exception yang tidak perlu di laporkan ke sentry
                if (ex is BusinessException or UnauthenticatedException or NotAllowedException)
                {
                    throw;
                }
                
                // Tangkap dan laporkan kesalahan ke Sentry
                SentrySdk.CaptureException(ex);
                throw;
            }
        }
    }
}

