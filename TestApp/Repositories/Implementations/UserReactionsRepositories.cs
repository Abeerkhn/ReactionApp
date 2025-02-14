using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TestApp.DbContext;
using TestApp.Model;
using TestApp.Repositories.Interfaces;


namespace TestApp.Repositories
{
    public class UserReactionsRepository:IUserReactionsRepositories
    {
        private readonly IWebHostEnvironment _env;
        private readonly MainContext _context;

        public UserReactionsRepository(IWebHostEnvironment env, MainContext context)
        {
            _env = env;
            _context = context;
        }
        // 1. Get all user IDs who reacted on a specific video
        public async Task<List<UserReactionDto>> GetUsersWhoReactedOnVideoAsync(long videoId)
        {
            return await _context.UserReactions
                .Where(r => r.VideoId == videoId)
                .Include(r => r.Users) // Ensure Users is included
                .Select(r => new UserReactionDto
                {
                    FirstName = r.Users.FirstName,
                    ReactionId = r.UserId.Value
                })
                .Distinct()
                .ToListAsync();
        }



        // 2. Get all reactions of a specific user for a given video
        public async Task<List<UserReactions>> GetReactionsByUserAsync(long userId, long videoId)
        {
            return await _context.UserReactions
                .Where(r => r.UserId == userId && r.VideoId == videoId)
                .ToListAsync();
        }

        // 3. Get the reaction video URL for a specific reaction
        public async Task<string> GetReactionVideoUrlAsync(long reactionId)
        {
            var reaction = await _context.UserReactions
                .FirstOrDefaultAsync(r => r.Id == reactionId);

            return reaction?.ReactionUrl;
        }
    
    public async Task<string> SaveReactionVideoAsync(long userId, long videoId, string videoUrl)
        {
            try
            {
                
                // Save reaction details in the database
                var reaction = new UserReactions
                {
                    UserId = userId,
                    VideoId = videoId,
                    ReactionUrl =videoUrl ,
                    ReactedAt = DateTime.UtcNow
                };

                _context.UserReactions.Add(reaction);
                await _context.SaveChangesAsync();

                return reaction.ReactionUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving reaction: {ex.Message}");
                return null;
            }
        }
    }
}
public class UserReactionDto
{
    public string FirstName { get; set; }
    public long ReactionId { get; set; }
}
