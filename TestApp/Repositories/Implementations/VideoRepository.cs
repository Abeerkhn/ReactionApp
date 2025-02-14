using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApp.DbContext;
using TestApp.Model;

public class VideoRepository : IVideoRepository
{
    private readonly MainContext _context;

    public VideoRepository(MainContext context)
    {
        _context = context;
    }

    public async Task AddVideoAsync(Videos video)
    {
        _context.Videos.Add(video);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteVideoAsync(long videoId)
    {

        try
        {
            var result= false;

        var video =   await   _context.Videos.AsQueryable().Where(x=>x.Id==videoId).FirstOrDefaultAsync();
            if(video is not null)
            {
                 
                var reactionlist = await _context.UserReactions.AsQueryable() .Where(x=>x.VideoId==videoId).ToListAsync();
                if(reactionlist is not null)
                {
                    _context.RemoveRange(reactionlist);
                }
                _context.Remove(video);
                result = true;
               
            }
            await _context.SaveChangesAsync();

            return result;
        }
        catch (Exception ex) {
            return false;
        }

    }

    public async Task<List<Videos>> GetAllVideosAsync()
    {
        return await _context.Videos.ToListAsync();
    }

    public async Task<Videos> GetVideoByIdAsync(string videoId)
    {
        // Try to parse the videoId string to a long
        if (long.TryParse(videoId, out long parsedVideoId))
        {
            // If parsing is successful, query the database with the parsed long value
            return await _context.Videos
                .Where(x => x.Id == parsedVideoId)
                .FirstOrDefaultAsync();
        }
        else
        {
            // If parsing fails, handle the error (e.g., return null or throw an exception)
            return null; // or throw new ArgumentException("Invalid
        }
    }
}