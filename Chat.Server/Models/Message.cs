namespace Chat.Server.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Is_System { get; set; }
        public bool Is_Private { get; set; }
    }
}
