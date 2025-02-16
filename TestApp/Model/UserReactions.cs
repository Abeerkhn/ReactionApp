namespace TestApp.Model
{
    public class UserReactions
    {
        public long Id { get; set; }
        public long? VideoId { get; set; }
        public long? UserId { get; set; }
        public string? ReactionUrl { get; set; }

        public DateTime? ReactedAt { get; set; }
        public Videos? Videos { get; set; }
        public Users? Users { get; set; }
        // Ensure Survey is Attempted
        public bool HasAttemptedSurvey { get; set; } = false;

        // ✅ List of User Survey Responses related to this reaction
        public List<UserSurveyResponses>? SurveyResponses { get; set; }
    }

}
