using System;

namespace WebApplication1.Models
{
    public enum ActivityType
    {
        ImageUpload,
        CreateStudent,
        EditStudent,
        DeleteStudent,
        ViewStudent,
        ListStudents
    }

    public class UserActivity
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public ActivityType ActivityType { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string AdditionalData { get; set; } = string.Empty;
    }
} 