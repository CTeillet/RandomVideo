using Microsoft.AspNetCore.Mvc;
using RandomVideo.Service;
using Xabe.FFmpeg;

namespace RandomVideo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController(
    IVideoService videoService)
    : ControllerBase
{

    [HttpGet]
    public IActionResult GetVideos()
    {
        return Ok(videoService.GetVideoAndCreateThumbnails());
    }

    [HttpGet("{fileName}")]
    public IActionResult GetVideo(string fileName)
    {
        return new FileStreamResult(videoService.GetVideo(fileName), "video/mp4");
    }

}