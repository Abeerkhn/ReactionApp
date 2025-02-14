using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Repositories.Interfaces;
using TestApp.Model;
using TestApp.Repositories;

namespace TestApp.Pages
{
    public class VideoRecorderModel : PageModel
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IUserReactionsRepositories userReactionsRepositories;

        public VideoRecorderModel(IVideoRepository videoRepository,IUserReactionsRepositories userReactionsRepositories, IWebHostEnvironment webHostEnvironment)
        {
            _videoRepository = videoRepository;
            _env = webHostEnvironment;
            this.userReactionsRepositories = userReactionsRepositories;
                
        }

        public Videos SelectedVideo { get; set; }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUploadVideoAsync([FromForm] IFormFile videoFile, long userId, long videoId)
        {
            if (videoFile == null || videoFile.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file received." });
            }

            try
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "userreaction");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = $"{Guid.NewGuid()}.webm";
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                }

                string savedUrl = $"/userreaction/{fileName}";

                // Save record in the database AFTER the file is successfully stored
                await userReactionsRepositories.SaveReactionVideoAsync(userId, videoId, savedUrl);

                return new JsonResult(new { success = true, videoUrl = savedUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }
        public async Task<IActionResult> OnGetAsync(string videoId)
        {
            if (string.IsNullOrEmpty(videoId))
            {
                return NotFound(); // Handle missing video ID
            }

            // Fetch video from repository
            SelectedVideo = await _videoRepository.GetVideoByIdAsync(videoId);

            if (SelectedVideo == null)
            {
                return NotFound(); // Handle invalid video ID
            }

            if (SelectedVideo.VideoType == VideoType.YouTube)
            {
                SelectedVideo.VideoUrl = ConvertToEmbedUrl(SelectedVideo.VideoUrl);
            }
            else if (SelectedVideo.VideoType == VideoType.Uploaded)
            {
                SelectedVideo.VideoUrl = $"{Request.Scheme}://{Request.Host}/{SelectedVideo.VideoUrl.Replace("\\", "/")}";
            }

            return Page();
        }

        public string ExtractYouTubeVideoId(string url)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/\s]{11})");
            var match = regex.Match(url);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private string ConvertToEmbedUrl(string videoUrl)
        {
            var videoId = ExtractYouTubeVideoId(videoUrl);
            return !string.IsNullOrEmpty(videoId) ? $"https://www.youtube.com/embed/{videoId}" : videoUrl;
        }


        //private string ConvertToEmbedUrl(string videoUrl)
        //{
        //    if (videoUrl.Contains("youtu.be"))
        //    {
        //        var videoId = videoUrl.Split('/').Last().Split('?')[0];
        //        return $"https://www.youtube.com/embed/{videoId}";
        //    }
        //    else if (videoUrl.Contains("youtube.com/watch?v="))
        //    {
        //        var videoId = videoUrl.Split("v=").Last().Split('&')[0];
        //        return $"https://www.youtube.com/embed/{videoId}";
        //    }
        //    return videoUrl; // Return as is if no match
        //}
    }
}
