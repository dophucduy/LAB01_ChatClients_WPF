namespace Chat.Server.Models
{
    public class Reaction
    {
        public int Id { get; set; }
        public int Message_Id { get; set; }
        public string Username { get; set; }
        public string Reaction_Type { get; set; }
        public DateTime Created_At { get; set; }
    }

}
