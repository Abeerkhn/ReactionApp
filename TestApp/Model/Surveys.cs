namespace TestApp.Model
{
    public class Surveys
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }

        // Navigation Properties
        public Users? CreatedByUser { get; set; }
        public ICollection<SurveyQuestions>? Questions { get; set; }

        // One-to-One Relationship with Videos
        public long? VideoId { get; set; } // Required: Every survey belongs to a video
        public Videos? Video { get; set; }
    }
}
