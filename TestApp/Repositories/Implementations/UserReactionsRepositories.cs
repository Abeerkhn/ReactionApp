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
    
    public async Task<long> SaveReactionVideoAsync(long userId, long videoId, string videoUrl)
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

                return reaction.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving reaction: {ex.Message}");
                return 0;
            }
        }

        public async Task SaveSurveyResponsesAsync(long userId, long reactionId, List<UserSurveyResponseDto> responses)
        {
            var surveyResponses = responses.Select(dto => new UserSurveyResponses
            {
                UserId = userId,
                ReactionId = reactionId,
                QuestionId = dto.QuestionId,
                SelectedAnswerId = dto.SelectedAnswerId,
                AnsweredAt = DateTime.UtcNow
            }).ToList();

            _context.UserSurveyResponses.AddRange(surveyResponses);
            await _context.SaveChangesAsync();
        }


        public async Task<UserReactions> GetReactionWithSurveyResponsesAsync(long reactionId)
        {
            return await _context.UserReactions
        .Where(r => r.Id == reactionId)
        .Include(r => r.SurveyResponses)
            .ThenInclude(sr => sr.Question)
        .Include(r => r.SurveyResponses)
            .ThenInclude(sr => sr.SelectedAnswer)
        .FirstOrDefaultAsync();
        }
    }
}
public class UserSurveyResponseDto
{
    public long QuestionId { get; set; }
    public long SelectedAnswerId { get; set; }
}

public class UserReactionDto
{
    public string FirstName { get; set; }
    public long ReactionId { get; set; }
}
