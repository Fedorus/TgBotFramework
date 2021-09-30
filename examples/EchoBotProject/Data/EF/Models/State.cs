using System;
using TgBotFramework;

namespace EchoBotProject.Data.EF.Models
{
    public class State
    {
        public long StateId { get; set; }
        
        public virtual User User { get; set; }
        public virtual GroupChat GroupChat { get; set; }
        
        public string Stage { get; set; } 
        public long Step { get; set; }
        public Role Role { get; set; }
        public string Language { get; set; }
    }
}