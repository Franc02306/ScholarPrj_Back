using ScholarPrj_Back.Domain.Responses.Common;

namespace ScholarPrj_Back.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                var statusCode = StatusCodes.Status500InternalServerError;
                var message = "Ocurrió un error interno.";

                // errores de validación
                if (ex is ArgumentException || ex is InvalidOperationException)
                {
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                }

                // errores de acceso no autorizado
                if (ex is UnauthorizedAccessException)
                {
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = "No autorizado.";
                }

                context.Response.StatusCode = statusCode;

                var response = new ApiResponse<object>
                {
                    Success = false,
                    Message = message,
                    Data = null
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}