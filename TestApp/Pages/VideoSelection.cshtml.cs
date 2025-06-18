using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Repositories.Interfaces;
using TestApp.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace TestApp.Pages
{
    public class VideoSelectionModel : PageModel
    {
        private readonly IVideoRepository _videoRepository;

        public VideoSelectionModel(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public List<Videos> AllVideos { get; set; } = new List<Videos>();

        public async Task<IActionResult> OnGetAsync()
        {
            AllVideos = await _videoRepository.GetAllVideosAsync();

            // Convert YouTube short URLs to embeddable format
            foreach (var video in AllVideos)
            {
                if (video.VideoType == VideoType.YouTube && video.VideoUrl.Contains("youtu.be"))
                {
                    var videoId = video.VideoUrl.Split('/').Last().Split('?')[0]; // Extract video ID
                    video.VideoUrl = $"https://www.youtube.com/embed/{videoId}"; // Convert to embed URL
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // sign out of the cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // redirect to the Login page
            return RedirectToPage("/Login");
        }
    }
}
