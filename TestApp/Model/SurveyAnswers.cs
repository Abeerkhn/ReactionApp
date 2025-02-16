namespace TestApp.Model
{
    public class SurveyAnswers
    {
        public long Id { get; set; }
        public long QuestionId { get; set; } // Foreign Key to SurveyQuestions
        public string AnswerText { get; set; } // Answer Option
        public bool IsCorrect { get; set; } // Indicates if this is the correct answer

        // Navigation Properties
        public SurveyQuestions? Question { get; set; }
        // ✅ Add Missing Navigation Property
        public ICollection<UserSurveyResponses>? UserResponses { get; set; }
    }

}
