using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace Chat.Server.Models
{
    [Table("messages")]
    public class Message : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        
        [Column("sender_id")]
        public int Sender_Id { get; set; }
        
        [Column("content")]
        public string Content { get; set; }
        
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        
        [Column("is_private")]
        public bool Is_Private { get; set; }
        
        [Column("receiver_id")]
        public int? Receiver_Id { get; set; }
    }
}
