namespace Chat.Server.Models
{
    public class UserInfo
    {
        public string ConnectionId { get; set; }
        public string Username { get; set; }
        public DateTime ConnectedAt { get; set; }
    }
}
