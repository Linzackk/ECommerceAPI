using ECommerce.Exceptions;

namespace ECommerce.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private static Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            int statusCode = StatusCodes.Status500InternalServerError;
            string message = "Erro interno do Servidor.";

            if (ex is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                message = ex.Message;
            }
            else if (ex is ParametroInvalidoException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
            }

            context.Response.StatusCode = statusCode;
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                error = message,
                status = statusCode
            });

            return context.Response.WriteAsync(result);
        }
    }
}
