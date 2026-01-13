namespace Chat.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Created_At { get; set; }
        public bool Is_Online { get; set; }
    }
}
