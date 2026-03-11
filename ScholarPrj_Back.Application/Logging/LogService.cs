using Microsoft.Extensions.Options;
using ScholarPrj_Back.Domain.Enums;
using ScholarPrj_Back.Infrastructure.Configuration;

namespace ScholarPrj_Back.Application.Logging
{
    public class LogService : ILogService
    {
        private readonly ModuleLogging _settings;

        public LogService(IOptions<ModuleLogging> options)
        {
            _settings = options.Value;
        }

        /// <summary>
        /// Genera el archivo de log según el módulo
        /// </summary>
        public async Task WriteAsync(LogEntry entry)
        {
            var basePath = ResolveBasePath(entry.Module);
            Directory.CreateDirectory(basePath);

            var moduleName = entry.Module.ToLower();
            var fileName = $"{moduleName}-{DateTime.Now:yyyy-MM-dd}.log";
            var fullPath = Path.Combine(basePath, fileName);

            var line = Format(entry);
            await File.AppendAllTextAsync(fullPath, line + Environment.NewLine);
        }

        /// <summary>
        /// Método que realiza decide a que módulo pertenece el log
        /// </summary>
        private string ResolveBasePath(string module)
        {
            if (module.Equals("Users", StringComparison.OrdinalIgnoreCase))
                return _settings.UserBasePath;

            return Path.Combine(_settings.BasePath, module);
        }

        /// <summary>
        /// Formatea la información del LogEntry en una sola línea legible
        /// </summary>
        private static string Format(LogEntry e)
        {
            return
                $"[{e.Date:yyyy-MM-dd HH:mm:ss.fff}] " +
                $"[{e.Module}] [{e.Method}] [{e.Path}] " +
                $"[{e.Action}] [{e.StatusCode}] " +
                $"[User={e.User ?? "anon"}] " +
                $"[Request={e.RequestBody ?? "<empty>"}] " +
                $"{(string.IsNullOrEmpty(e.ResponseBody) ? string.Empty : $"[Response={e.ResponseBody}] ")}" +
                $"{(string.IsNullOrEmpty(e.Error) ? string.Empty : $"Error={e.Error}")}";
        }
    }
}
