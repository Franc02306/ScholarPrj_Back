namespace ScholarPrj_Back.Domain.Enums
{
    public class LogEntry
    {
        public DateTime Date { get; set; }
        public string Module { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string Method { get; set; } = default!;
        public string Path { get; set; } = default!;
        public int StatusCode { get; set; }
        public string? User { get; set; }
        public string? Error { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
    }
}
