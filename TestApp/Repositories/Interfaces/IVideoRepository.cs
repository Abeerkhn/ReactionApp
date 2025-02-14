using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Model;

public interface IVideoRepository
{
    Task AddVideoAsync(Videos video);
    Task<List<Videos>> GetAllVideosAsync();
    Task<Videos> GetVideoByIdAsync(string videoId);
    Task<bool> DeleteVideoAsync(long videoId);

}
