using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TestApp.Model;

namespace TestApp.DbContext
{
    public class MainContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public MainContext(DbContextOptions<MainContext> options) : base(options) { }

        // Define DbSets for each entity
        public DbSet<Users> Users { get; set; }
        public DbSet<Videos> Videos { get; set; }
        public DbSet<UserReactions> UserReactions { get; set; }

        public DbSet<Surveys> Surveys { get; set; }
        public DbSet<SurveyQuestions> SurveyQuestions { get; set; }
        public DbSet<SurveyAnswers> SurveyAnswers { get; set; }
        public DbSet<UserSurveyResponses> UserSurveyResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-Videos One-to-Many (A user uploads videos)
            modelBuilder.Entity<Videos>()
                .HasOne(v => v.Users)
                .WithMany(u => u.UploadedVideos)
                .HasForeignKey(v => v.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict); // Prevents multiple cascade paths issue

            // User-UserReactions One-to-Many 
            modelBuilder.Entity<UserReactions>()
                .HasOne(ur => ur.Users)
                .WithMany(u => u.Reactions)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Delete reactions when a user is deleted

            // Video-UserReactions One-to-Many 
            modelBuilder.Entity<UserReactions>()
                .HasOne(ur => ur.Videos)
                .WithMany(v => v.Reactions)
                .HasForeignKey(ur => ur.VideoId)
                .OnDelete(DeleteBehavior.Restrict); // 🚫 Prevents cascade path issue

            // Video-Survey One-to-One (Each video has at most one survey)
            modelBuilder.Entity<Videos>()
                .HasOne(v => v.Survey)
                .WithOne(s => s.Video)
                .HasForeignKey<Surveys>(s => s.VideoId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Delete survey when video is deleted

            // Survey-SurveyQuestions One-to-Many (Each survey has multiple questions)
            modelBuilder.Entity<SurveyQuestions>()
                .HasOne(q => q.Survey)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Delete questions when a survey is deleted

            // SurveyQuestions-SurveyAnswers One-to-Many (Each question has four answers)
            modelBuilder.Entity<SurveyAnswers>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Delete answers when a question is deleted

            // User-SurveyResponses One-to-Many (A user attempts multiple surveys)
            modelBuilder.Entity<UserSurveyResponses>()
                .HasOne(usr => usr.User)
                .WithMany(u => u.SurveyResponses)
                .HasForeignKey(usr => usr.UserId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Delete responses when a user is deleted

            // SurveyQuestion-UserSurveyResponses One-to-Many (Each response is for a question)
            modelBuilder.Entity<UserSurveyResponses>()
                .HasOne(usr => usr.Question)
                .WithMany(q => q.UserResponses)
                .HasForeignKey(usr => usr.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // 🚫 Prevents cascade path issue

            // SurveyAnswer-UserSurveyResponses One-to-One (Each response selects an answer)
            modelBuilder.Entity<UserSurveyResponses>()
                .HasOne(usr => usr.SelectedAnswer)
                .WithMany(a => a.UserResponses)
                .HasForeignKey(usr => usr.SelectedAnswerId)
                .OnDelete(DeleteBehavior.Restrict); // 🚫 Prevents cascade path issue
                                                    // ✅ One-to-Many: UserReactions → UserSurveyResponses (Each reaction has multiple responses)
            modelBuilder.Entity<UserSurveyResponses>()
                .HasOne(usr => usr.Reaction)
                .WithMany(ur => ur.SurveyResponses) // ✅ Each reaction has multiple survey responses
                .HasForeignKey(usr => usr.ReactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }



    }
}
