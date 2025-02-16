namespace TestApp.Model
{
    public class Users
    {
        public long Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Password { get; set; }
        public string? EmailOrPhoneNumber { get; set; }
        public string? Image { get; set; }

        public DateTime? CreatedDate { get; set; }
        public long? CreatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedById { get; set; }

        public ICollection<Videos>? UploadedVideos { get; set; }
        public ICollection<UserReactions>? Reactions { get; set; }

        // ✅ Add Missing Navigation Property
        public ICollection<UserSurveyResponses>? SurveyResponses { get; set; }
    }
}
