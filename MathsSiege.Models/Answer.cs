namespace MathsSiege.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public int ChoiceId { get; set; }

        public GameSession GameSession { get; set; }
        public Choice Choice { get; set; }
    }
}