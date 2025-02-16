namespace TestApp.Model
{
   
        public enum VideoType
        {
            Uploaded = 1,
            YouTube,
            Recorded
        }
        public class Videos
        {
            public long Id { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public VideoType VideoType { get; set; }
            public string? VideoUrl { get; set; }
            public DateTime? UploadedDate { get; set; }
            public long? UploadedBy { get; set; }
            // Navigation Property
            public Users? Users { get; set; }
            public ICollection<UserReactions>? Reactions { get; set; }

        // One-to-One Relationship with Surveys
        public long? SurveyId { get; set; } // Nullable: Some videos may not have surveys
        public Surveys? Survey { get; set; }
    }
    }

