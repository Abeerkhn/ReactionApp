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


        [BindProperty]
        public long UserId { get; set; }

        [BindProperty]
        public long ReactionId { get; set; }

        [BindProperty]
        public System.Collections.Generic.List<UserSurveyResponseDto> SurveyResponses { get; set; }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSubmitSurveyAsync()
        {
            if (SurveyResponses == null || !SurveyResponses.Any())
            {
                return BadRequest(new { success = false, message = "No responses received." });
            }

            try
            {
                long userId = UserId;       // Bound from hidden field
                long reactionId = ReactionId;  // Bound from hidden field

                // Save responses via repository (make sure this method is implemented)
                 await userReactionsRepositories.SaveSurveyResponsesAsync(userId, reactionId, SurveyResponses);
                return RedirectToPage("/VideoSelection");
            //    return new JsonResult(new { success = true, message = "Survey submitted successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        // [ValidateAntiForgeryToken]
        //public async Task<IActionResult> OnPostSubmitSurveyAsync([FromForm] List<UserSurveyResponseDto> request, long UserId)
        //{
        //    if (request == null || request == null || !request.Any())
        //    {
        //        return BadRequest(new { success = false, message = "No responses received." });
        //    }

        //    try
        //    {
        //        long userId = request.UserId;
        //        long reactionId = request.ReactionId;

        //        // Save responses via repository (make sure this method is implemented)
        //        // await _userReactionsRepositories.SaveSurveyResponsesAsync(userId, reactionId, request.SurveyResponses);

        //        return new JsonResult(new { success = true, message = "Survey submitted successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return StatusCode(500, new { success = false, message = "Internal server error." });
        //    }
        //}

        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> OnPostUploadVideoAsync([FromForm] IFormFile videoFile, long userId, long videoId)
        //{
        //    if (videoFile == null || videoFile.Length == 0)
        //    {
        //        return BadRequest(new { success = false, message = "No file received." });
        //    }

        //    try
        //    {
        //        string uploadPath = Path.Combine(_env.WebRootPath, "userreaction");
        //        if (!Directory.Exists(uploadPath))
        //        {
        //            Directory.CreateDirectory(uploadPath);
        //        }

        //        string fileName = $"{Guid.NewGuid()}.webm";
        //        string filePath = Path.Combine(uploadPath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await videoFile.CopyToAsync(stream);
        //        }

        //        string savedUrl = $"/userreaction/{fileName}";

        //        // Save record in the database AFTER the file is successfully stored
        //        long reactionId = await userReactionsRepositories.SaveReactionVideoAsync(userId, videoId, savedUrl);
        //       // UserId = userId;
        //        //ReactionId = reactionId;
        //        return new JsonResult(new { success = true, videoUrl = savedUrl, reactionId=reactionId });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Server error: {ex.Message}");
        //        return StatusCode(500, new { success = false, message = "Internal server error." });
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> OnPostUploadChunkAsync(IFormFile videoChunk, long userId, long videoId, int chunkIndex, bool isLastChunk)
        {
            if (videoChunk == null || videoChunk.Length == 0)
            {
                return BadRequest(new { success = false, message = "No chunk received." });
            }

            try
            {
                string userFolder = Path.Combine(_env.WebRootPath, "userreaction", $"video_{videoId}_user_{userId}");
                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder);
                }

                string chunkFilePath = Path.Combine(userFolder, $"chunk_{chunkIndex}.webm");

                using (var stream = new FileStream(chunkFilePath, FileMode.Create))
                {
                    await videoChunk.CopyToAsync(stream);
                }

                // Combine chunks if it's the last one
                if (isLastChunk)
                {
                    string finalVideoPath = Path.Combine(_env.WebRootPath, "userreaction", $"{Guid.NewGuid()}.webm");

                    using (var finalStream = new FileStream(finalVideoPath, FileMode.Create))
                    {
                        int i = 0;
                        while (true)
                        {
                            string chunkPath = Path.Combine(userFolder, $"chunk_{i}.webm");
                            if (!System.IO.File.Exists(chunkPath))
                                break;

                            byte[] chunkBytes = await System.IO.File.ReadAllBytesAsync(chunkPath);
                            await finalStream.WriteAsync(chunkBytes, 0, chunkBytes.Length);
                            i++;
                        }
                    }

                    // Optionally delete the chunks after merging
                    Directory.Delete(userFolder, true);

                    string savedUrl = $"/userreaction/{Path.GetFileName(finalVideoPath)}";
                    long reactionId = await userReactionsRepositories.SaveReactionVideoAsync(userId, videoId, savedUrl);

                    return new JsonResult(new { success = true, videoUrl = savedUrl, reactionId });
                }

                return new JsonResult(new { success = true }); // intermediate chunk uploaded
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


        
    }
}

public class SurveySubmissionRequest
{
    
    public long UserId { get; set; }
    public long ReactionId { get; set; }
    public List<UserSurveyResponseDto> SurveyResponses { get; set; }
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