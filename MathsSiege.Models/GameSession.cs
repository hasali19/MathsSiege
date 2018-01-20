using System;
using System.Collections.Generic;

namespace MathsSiege.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public User User { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
