using TestApp.Model;

namespace TestApp.Repositories.Interfaces
{
    public interface IUserReactionsRepositories
    {
        Task<string> SaveReactionVideoAsync(long userId, long videoId, string url);
        Task<List<UserReactionDto>> GetUsersWhoReactedOnVideoAsync(long videoId);
        Task<List<UserReactions>> GetReactionsByUserAsync(long userId, long videoId);
        Task<string> GetReactionVideoUrlAsync(long reactionId);
    }
}
