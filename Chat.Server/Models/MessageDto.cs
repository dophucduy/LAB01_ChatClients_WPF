namespace Chat.Server.Models
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Is_System { get; set; }
        public bool Is_Private { get; set; }
        public string Recipient { get; set; }
        
        // File message properties
        public bool IsFile { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public long FileSize { get; set; }
    }
}
