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
        [HttpPost]
        public async Task<IActionResult> OnPostUploadChunkAsync(IFormFile videoChunk, long userId, long videoId, int chunkIndex, [FromForm] string isLastChunk, string sessionId)
        {
            if (videoChunk == null || videoChunk.Length == 0 || string.IsNullOrWhiteSpace(sessionId))
            {
                return BadRequest(new { success = false, message = "Invalid upload request" });
            }

            try
            {
                // 1. Folder per session ID
                string tempFolder = Path.Combine(_env.WebRootPath, "temp_chunks", sessionId);
                Directory.CreateDirectory(tempFolder);

                // 2. Unique chunk file name
                string chunkFileName = $"chunk_{chunkIndex}";
                string tempPath = Path.Combine(tempFolder, $"{chunkFileName}.tmp");
                string finalChunkPath = Path.Combine(tempFolder, $"{chunkFileName}.webm");

                // 3. Save chunk
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await videoChunk.CopyToAsync(stream);
                }
                System.IO.File.Move(tempPath, finalChunkPath);

                // 4. Final chunk = merge
                bool last = bool.TryParse(isLastChunk, out var b) && b;
                if (last)
                {
                    int expectedChunks = chunkIndex + 1;

                    for (int i = 0; i < expectedChunks; i++)
                    {
                        string chunkPath = Path.Combine(tempFolder, $"chunk_{i}.webm");
                        if (!System.IO.File.Exists(chunkPath))
                        {
                            return BadRequest(new { success = false, message = $"Missing chunk {i}. Upload failed." });
                        }
                    }

                    string finalFileName = $"{Guid.NewGuid()}.webm";
                    string finalVideoPath = Path.Combine(_env.WebRootPath, "userreaction", finalFileName);

                    await using (var finalStream = new FileStream(finalVideoPath, FileMode.Create))
                    {
                        for (int i = 0; i < expectedChunks; i++)
                        {
                            string chunkPath = Path.Combine(tempFolder, $"chunk_{i}.webm");
                            await using (var chunkStream = System.IO.File.OpenRead(chunkPath))
                            {
                                await chunkStream.CopyToAsync(finalStream);
                            }
                        }
                    }

                    if (new FileInfo(finalVideoPath).Length == 0)
                    {
                        System.IO.File.Delete(finalVideoPath);
                        return StatusCode(500, new { success = false, message = "Failed to merge video" });
                    }

                    string savedUrl = $"/userreaction/{finalFileName}";
                    long reactionId = await userReactionsRepositories.SaveReactionVideoAsync(userId, videoId, savedUrl);

                    try { Directory.Delete(tempFolder, true); } catch { }

                    return new JsonResult(new
                    {
                        success = true,
                        videoUrl = savedUrl,
                        reactionId
                    });
                }

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
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