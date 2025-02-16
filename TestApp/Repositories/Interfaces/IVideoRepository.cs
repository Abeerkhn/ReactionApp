using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Model;

public interface IVideoRepository
{
    Task AddVideoAsync(Videos video,List<SurveyQuestionDto> questions);
    Task<List<Videos>> GetAllVideosAsync();
    Task<Videos> GetVideoByIdAsync(string videoId);
    Task<bool> DeleteVideoAsync(long videoId);

}
