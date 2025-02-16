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

    public async Task AddVideoAsync(Videos video, List<SurveyQuestionDto> questions)
    {
        if (questions == null || questions.Count == 0)
        {
            throw new ArgumentException("Survey must have at least one question.");
        }

        // ✅ Step 1: Save the Video First
        _context.Videos.Add(video);
        await _context.SaveChangesAsync(); // Ensures video.Id is generated

        // ✅ Step 2: Create the survey and associate it with the video
        var survey = new Surveys
        {
            Title = "Survey for " + video.Title,
            Description = "Please answer the questions after watching this video.",
            VideoId = video.Id, // Now we have a valid video ID
            Questions = new List<SurveyQuestions>()
        };

        // ✅ Step 3: Add questions and answers to the survey
        foreach (var questionDto in questions)
        {
            var surveyQuestion = new SurveyQuestions
            {
                QuestionText = questionDto.QuestionText,
                Answers = new List<SurveyAnswers>()
            };

            foreach (var answerText in questionDto.Answers)
            {
                surveyQuestion.Answers.Add(new SurveyAnswers
                {
                    AnswerText = answerText
                });
            }

            survey.Questions.Add(surveyQuestion);
        }

        // ✅ Step 4: Save the survey after assigning the correct VideoId
        _context.Surveys.Add(survey);
        await _context.SaveChangesAsync();

        // ✅ Step 5: Assign the generated SurveyId to the Video and update the video record
        video.SurveyId = survey.Id;
        _context.Videos.Update(video);
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

                var reactionlist = await _context.UserReactions.AsQueryable().Where(x => x.VideoId == videoId).ToListAsync();
                
                if (reactionlist is not null)
                { foreach(var reaction in reactionlist)
                    {
                        var surveylist = await _context.UserSurveyResponses.AsQueryable().Where(x => x.ReactionId == reaction.Id).ToListAsync();
                        _context.RemoveRange(surveylist);
                    }
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
            return await _context.Videos.Include(x=>x.Survey).ThenInclude(x=>x.Questions).ThenInclude(x=>x.Answers)
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
public class SurveyQuestionDto
{
    public string QuestionText { get; set; }
    public List<string> Answers { get; set; }
}
