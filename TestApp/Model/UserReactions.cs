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

    }

}
