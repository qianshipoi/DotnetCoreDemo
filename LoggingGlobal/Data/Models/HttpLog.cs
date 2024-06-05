using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoggingGlobal.Data.Models
{
    public class HttpLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Protocol { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Scheme { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string? QueryString { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public int StatusCode { get; set; }
        public string? RequestHeaders { get; set; }
        public string? ResponseHeaders { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
