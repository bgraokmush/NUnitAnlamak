namespace InternEvaluation.Models
{
    public class InternApplicant
    {
        public Intern Intern { get; set; }
        public List<string> TeckList { get; set; }
        public int QuizSkore { get; set; }
        public bool isInterviewSuccess { get; set; }
    }
}
