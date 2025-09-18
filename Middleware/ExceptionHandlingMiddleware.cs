using System.Net;
using System.Text.Json; 
using ProductService.Exceptions;

namespace ProductService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger; 
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (NotFoundException nf)
            {
                _logger.LogWarning(nf, "Not found");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode= (int)HttpStatusCode.NotFound;
                var errorResponse = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = nf.Message
                };

                var options=new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught by middleware"); 
                context.Response.ContentType="application/json";
                context.Response.StatusCode=(int)HttpStatusCode.InternalServerError;

                var errorResponse = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An unexpected error occurred.",
                    Detailed = ex.Message // Serra masqué en prod
                };

                var options=new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    
            }

        }
    }
}