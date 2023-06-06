using System.ComponentModel.DataAnnotations;

namespace TreeNodes.Models
{
    public class ExceptionLog
    {
        [Key]
        public int Id { get; set; }
        public string? EventType { get; set; }
        public string? EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? QueryParameters { get; set; }
        public string? BodyParameters { get; set; }
        public string? StackTrace { get; set; }
    }
}
