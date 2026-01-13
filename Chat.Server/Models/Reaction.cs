using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Chat.Server.Models
{
    [Table("reactions")]
    public class Reaction : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        
        [Column("message_id")]
        public int Message_Id { get; set; }
        
        [Column("user_id")]
        public int User_Id { get; set; }
        
        [Column("reaction_type")]
        public string Reaction_Type { get; set; }
        
        [Column("created_at")]
        public DateTime Created_At { get; set; }
        
        // For display purposes only - not stored in DB
        public string Username { get; set; }
    }
}
