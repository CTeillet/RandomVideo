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
        var fileStream = videoService.GetVideo(fileName);
        var fileLength = fileStream.Length;
        var request = HttpContext.Request;

        if (!request.Headers.ContainsKey("Range"))
        {
            return File(fileStream, "video/mp4", enableRangeProcessing: true);
        }

        // Lire la range demandée
        var rangeHeader = request.Headers.Range.ToString();
        var range = rangeHeader.Replace("bytes=", "").Split('-');
        var start = long.Parse(range[0]);
        var end = range.Length > 1 && !string.IsNullOrEmpty(range[1]) ? long.Parse(range[1]) : fileLength - 1;
        var contentLength = end - start + 1;

        fileStream.Seek(start, SeekOrigin.Begin);
        var partialStream = new MemoryStream();
        fileStream.CopyTo(partialStream);
        partialStream.Position = 0;

        Response.StatusCode = 206; // Partial Content
        Response.Headers.Append("Content-Range", $"bytes {start}-{end}/{fileLength}");
        Response.Headers.Append("Accept-Ranges", "bytes");
        Response.ContentLength = contentLength;

        return File(partialStream, "video/mp4", enableRangeProcessing: false);
    }


}