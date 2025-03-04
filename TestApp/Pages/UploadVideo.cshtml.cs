using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TestApp.Repositories.Interfaces;
using TestApp.Model;
using Microsoft.AspNetCore.Http;
using System.Text;

public class UploadVideoModel : PageModel
{
    private readonly IVideoRepository _videoRepository;
    private  readonly IUserReactionsRepositories userReactionRepo;

    [BindProperty]
    public string Title { get; set; }

    [BindProperty]
    public string Description { get; set; }

    [BindProperty]
    public VideoType VideoType { get; set; } // Enum: Uploaded, YouTube, Recorded

    [BindProperty]
    public string VideoUrl { get; set; } // YouTube URL or local path

    [BindProperty]
    public IFormFile VideoFile { get; set; } // For file uploads

    public List<Videos> Videos { get; set; } = new List<Videos>(); // List of videos
    [BindProperty]
    public List<SurveyQuestionDto> Questions { get; set; } = new List<SurveyQuestionDto>();


    public UploadVideoModel(IVideoRepository videoRepository, IUserReactionsRepositories userReactionRepo)
    {
        _videoRepository = videoRepository;
        this.userReactionRepo = userReactionRepo;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Videos = await _videoRepository.GetAllVideosAsync(); // Fetch videos from repository
        return Page();
    }
    public async Task<JsonResult> OnGetUserReactionsAsync(long videoId)
    {
        var users = await userReactionRepo.GetUsersWhoReactedOnVideoAsync(videoId);

        if (users == null || !users.Any())
        {
            return new JsonResult(new { success = false, message = "No reactions found." });
        }

        return new JsonResult(new { success = true, users });
    }

    public async Task<JsonResult> OnGetReactionsByUserAsync(int userId,long videoid)
    {
        var reactions = await userReactionRepo.GetReactionsByUserAsync(userId,videoid);
        return new JsonResult(reactions);
    }

    public async Task<JsonResult> OnGetReactionVideoAsync(int reactionId)
    {
        var reactionVideoUrl = await userReactionRepo.GetReactionVideoUrlAsync(reactionId);
        return new JsonResult(reactionVideoUrl);
    }

    public async Task<IActionResult> OnGetGetSurveyResponsesAsync(long reactionId)
    {
        // Fetch the reaction with its survey responses (make sure your repository method is implemented)
        var reaction = await userReactionRepo.GetReactionWithSurveyResponsesAsync(reactionId);
        if (reaction == null || reaction.SurveyResponses == null || !reaction.SurveyResponses.Any())
        {
            return new JsonResult(new { success = false, message = "No survey responses found." });
        }

        // Map responses to an anonymous object with a flag indicating if the selected answer is correct
        var responses = reaction.SurveyResponses.Select(sr => new {
            questionText = sr.Question?.QuestionText,
            answerText = sr.SelectedAnswer?.AnswerText,
            isCorrect = sr.SelectedAnswer != null && sr.SelectedAnswer.IsCorrect
        });

        return new JsonResult(new { success = true, surveyResponses = responses });
    }



    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            ModelState.AddModelError("", "Title is required.");
            return Page();
        }

        string videoPath = null;

        if (VideoType == VideoType.Uploaded)
        {
            if (VideoFile == null || VideoFile.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a valid video file.");
                return Page();
            }

            // Ensure the uploads directory exists
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Create a unique filename
            string fileExtension = Path.GetExtension(VideoFile.FileName);
            string safeFileName = $"{Guid.NewGuid()}{fileExtension}";
            videoPath = Path.Combine("uploads", safeFileName);
            string fullPath = Path.Combine(uploadsFolder, safeFileName);

            // Save the file
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await VideoFile.CopyToAsync(fileStream);
            }
        }
        else if (VideoType == VideoType.YouTube)
        {
            if (string.IsNullOrWhiteSpace(VideoUrl))
            {
                ModelState.AddModelError("", "Please provide a YouTube video URL.");
                return Page();
            }

            videoPath = VideoUrl;
        }
        else
        {
            ModelState.AddModelError("", "Invalid video type.");
            return Page();
        }

        // ✅ Step 1: Create Video Entity
        var video = new Videos
        {
            Title = Title,
            Description = Description,
            VideoType = VideoType,
            VideoUrl = videoPath,
            UploadedDate = DateTime.UtcNow,
            UploadedBy = 1 // Assuming user ID is 1 for now
        };

        // ✅ Step 2: Handle Survey Questions
        var surveyQuestions = new List<SurveyQuestionDto>();

        if (Questions != null && Questions.Any())
        {
            foreach (var question in Questions)
            {
                List<SurveyAnswerDto> answers = new List<SurveyAnswerDto>();
                foreach (var answer in question.Answers)
                {
                    answers.Add(new SurveyAnswerDto
                    {
                        AnswerText = answer.AnswerText,
                        IsCorrect = answer.IsCorrect
                    });

                }
                var surveyQuestion = new SurveyQuestionDto
                {
                    QuestionText = question.QuestionText,
                    Answers = answers
                };

                surveyQuestions.Add(surveyQuestion);
            }
        }

        // ✅ Step 3: Save Video & Survey Data (Assuming _videoRepository supports saving questions too)
        await _videoRepository.AddVideoAsync(video, surveyQuestions);

        return RedirectToPage("/UploadVideo"); // Refresh the page after upload
    }

    public async Task<IActionResult> OnGetDownloadSurveyResponsesCsvAsync(long videoId)
    {
        // Retrieve survey responses for the given video.
        var responses = await userReactionRepo.GetSurveyResponsesByVideoAsync(videoId);

        if (responses == null || !responses.Any())
        {
            return Content("No survey responses found for this video.");
        }

        // Group responses by user.
        var groupedResponses = responses
            .GroupBy(r => r.UserName)
            .ToList();

        // Determine the maximum number of responses any user has.
        int maxResponsesPerUser = groupedResponses.Max(g => g.Count());

        var csvBuilder = new StringBuilder();

        // Build header row.
        var headerColumns = new List<string> { "User Name" };
        for (int i = 1; i <= maxResponsesPerUser; i++)
        {
            headerColumns.Add("Question " + i);
        }
        csvBuilder.AppendLine(string.Join(",", headerColumns));

        // Build a CSV row for each user.
        foreach (var group in groupedResponses)
        {
            var row = new List<string>();
            // Add username as the first column.
            row.Add(EscapeForCsv(group.Key));

            int questionNo = 1;
            // Order responses as needed (here using the order they appear).
            foreach (var response in group)
            {
                string resultText = response.IsCorrect ? "correct" : "incorrect";
                // Construct the cell text for the survey response.
                string cellText = $"Question {questionNo}: {response.QuestionText} {response.AnswerText} {resultText}";
                row.Add(EscapeForCsv(cellText));
                questionNo++;
            }
            // Fill any missing columns if the user has fewer responses.
            for (int i = questionNo; i <= maxResponsesPerUser; i++)
            {
                row.Add("");
            }

            csvBuilder.AppendLine(string.Join(",", row));
        }

        byte[] fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
        string fileName = $"SurveyResponses_{videoId}.csv";
        return File(fileBytes, "text/csv", fileName);
    }


    // Helper method to escape CSV fields.
    private string EscapeForCsv(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return "";
        }
        // If the field contains a comma or a quote, wrap it in quotes and escape inner quotes.
        if (field.Contains(",") || field.Contains("\""))
        {
            field = field.Replace("\"", "\"\"");
            return $"\"{field}\"";
        }
        return field;
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostDeleteVideoAsync(long videoId)
    {
        // Your delete logic here. For example:
        var success = await _videoRepository.DeleteVideoAsync(videoId);
        if (!success)
        {
            return new JsonResult(new { success = false, message = "Failed to delete video." });
        }
        return new JsonResult(new { success = true });
    }


}
