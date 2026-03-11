using ScholarPrj_Back.Application.Logging;
using ScholarPrj_Back.Domain.Enums;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace ScholarPrj_Back.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogService logService)
        {
            var startDate = DateTime.UtcNow;

            context.Request.EnableBuffering();

            string? requestBody = null;

            using (var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            requestBody = HideSensitiveFields(requestBody);

            var originalResponseBody = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                await logService.WriteAsync(new LogEntry
                {
                    Date = startDate,
                    Module = ResolveModule(context),
                    Action = ResolveAction(context),
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    StatusCode = context.Response.StatusCode,
                    User = ResolveUser(context),
                    RequestBody = string.IsNullOrWhiteSpace(requestBody) ? null : requestBody,
                    ResponseBody = string.IsNullOrWhiteSpace(responseText) ? null : responseText
                });

                await responseBody.CopyToAsync(originalResponseBody);
            }
            catch (Exception ex)
            {
                await logService.WriteAsync(new LogEntry
                {
                    Date = startDate,
                    Module = ResolveModule(context),
                    Action = ResolveAction(context),
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    StatusCode = 500,
                    User = ResolveUser(context),
                    RequestBody = string.IsNullOrWhiteSpace(requestBody) ? null : requestBody,
                    Error = ex.Message
                });

                throw;
            }
            finally
            {
                context.Response.Body = originalResponseBody;
            }
        }

        private static string ResolveModule(HttpContext context)
        {
            var segments = context.Request.Path.Value?
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments == null || segments.Length < 2)
                return "General";

            return segments[1];
        }

        private static string ResolveAction(HttpContext context)
        {
            return context.GetEndpoint()?.DisplayName ?? "UnknownAction";
        }

        private static string? ResolveUser(HttpContext context)
        {
            return context.User?.FindFirst(ClaimTypes.Name)?.Value;
        }

        private static string? HideSensitiveFields(string? body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return body;

            var pattern = "\"(password|currentPassword|newPassword|token)\"\\s*:\\s*\".*?\"";

            return Regex.Replace(
                body,
                pattern,
                m =>
                {
                    var field = m.Groups[1].Value;
                    return $"\"{field}\":\"***\"";
                },
                RegexOptions.IgnoreCase
            );
        }
    }
}