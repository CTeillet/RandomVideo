using Microsoft.AspNetCore.Mvc;

namespace RandomVideo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly string videoDirectory = @"C:\Users\teill\Videos";

    [HttpGet]
    public ActionResult<IEnumerable<string>> GetVideos()
    {
        if (!Directory.Exists(videoDirectory))
        {
            return NotFound("Video directory not found");
        }

        var videos = Directory.EnumerateFiles(videoDirectory, "*.mp4", SearchOption.AllDirectories)
            .Select(s => Path.GetRelativePath(videoDirectory, s))
            .ToList();

        return Ok(videos);
    }

    [HttpGet("{fileName}")]
    public IActionResult GetVideo(string fileName)
    {
        Console.Out.WriteLine("fileName: " + fileName);
        var filePath = Path.Combine(videoDirectory, fileName);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return new FileStreamResult(stream, "video/mp4");
    }
}