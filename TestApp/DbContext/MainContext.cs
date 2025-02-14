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
        }


    }
}
