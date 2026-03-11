using ScholarPrj_Back.Domain.Enums;

namespace ScholarPrj_Back.Application.Logging
{
    public interface ILogService
    {
        /// <summary>
        /// Función que permite registrar un log de las acciones que realiza el backend
        /// </summary>
        Task WriteAsync(LogEntry entry);
    }
}
