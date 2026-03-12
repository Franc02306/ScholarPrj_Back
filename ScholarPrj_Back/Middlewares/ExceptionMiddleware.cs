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

                int statusCode;
                string message;

                switch (ex)
                {
                    // ERRORES INTERNOS (ocultar)
                    case ArgumentNullException:
                    case NullReferenceException:
                    case IOException:
                        statusCode = StatusCodes.Status500InternalServerError;
                        message = "Ocurrió un error interno.";
                        break;

                    // ERRORES DE NEGOCIO (mostrar)
                    case ArgumentException:
                    case InvalidOperationException:
                        statusCode = StatusCodes.Status400BadRequest;
                        message = ex.Message;
                        break;

                    case UnauthorizedAccessException:
                        statusCode = StatusCodes.Status401Unauthorized;
                        message = ex.Message;
                        break;

                    default:
                        statusCode = StatusCodes.Status500InternalServerError;
                        message = "Ocurrió un error interno.";
                        break;
                }

                context.Response.StatusCode = statusCode;

                var response = ApiResponse<object>.Fail(message);

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}