namespace TestApp.Model
{
    public class SurveyQuestions
    {
        public long Id { get; set; }
        public long SurveyId { get; set; } // Foreign Key to Surveys
        public string QuestionText { get; set; }

        // Navigation Properties
        public Surveys? Survey { get; set; }
        public ICollection<SurveyAnswers>? Answers { get; set; }

        // ✅ Add Missing Navigation Property
        public ICollection<UserSurveyResponses>? UserResponses { get; set; }
    }

}
