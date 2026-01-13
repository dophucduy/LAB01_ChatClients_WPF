using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Chat.Server.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        
        [Column("username")]
        public string Username { get; set; }
        
        [Column("created_at")]
        public DateTime Created_At { get; set; }
        
        [Column("is_online")]
        public bool Is_Online { get; set; }
    }
}
