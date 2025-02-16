namespace TestApp.Model
{
    public class UserSurveyResponses
    {
        public long Id { get; set; }
        public long UserId { get; set; } // Foreign Key to Users
        public long QuestionId { get; set; } // Foreign Key to SurveyQuestions
        public long SelectedAnswerId { get; set; } // Foreign Key to SurveyAnswers
        public DateTime AnsweredAt { get; set; }
        public long ReactionId { get; set; }

        // Navigation Properties
        public UserReactions? Reaction { get; set; } // Link to the reaction
        public Users? User { get; set; }
        public SurveyQuestions? Question { get; set; }
        public SurveyAnswers? SelectedAnswer { get; set; }
    }

}
