using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Client.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Is_System { get; set; }
        public bool Is_Private { get; set; }
        public Dictionary<string, int> Reactions { get; set; } = new Dictionary<string, int>();

    }
}
