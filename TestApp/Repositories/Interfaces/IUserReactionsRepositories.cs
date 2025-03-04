using TestApp.Model;

namespace TestApp.Repositories.Interfaces
{
    public interface IUserReactionsRepositories
    {
        Task<long> SaveReactionVideoAsync(long userId, long videoId, string videoUrl);

        Task SaveSurveyResponsesAsync(long userId, long reactionId, List<UserSurveyResponseDto> responses);
        Task<List<UserReactionDto>> GetUsersWhoReactedOnVideoAsync(long videoId);
        Task<List<UserReactions>> GetReactionsByUserAsync(long userId, long videoId);
        Task<string> GetReactionVideoUrlAsync(long reactionId);
        Task<UserReactions> GetReactionWithSurveyResponsesAsync(long reactionId);
        Task<List<SurveyResponseCsvDto>> GetSurveyResponsesByVideoAsync(long videoId);
    }
}
