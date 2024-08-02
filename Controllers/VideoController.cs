using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RandomVideo.Components.Pages;
using RandomVideo.Models;
using RandomVideo.Service;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace RandomVideo.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
public class VideoController(
    IVideoThumbnailGenerator thumbnailGenerator,
    IListVideo listVideo,
    IConfiguration configuration)
    : ControllerBase
{
    private readonly string _videoDirectory = configuration["PathVideoDirectory"] ?? throw new ArgumentException("PathVideoDirectory is not configured in appsettings.json");

    [HttpGet]
    public IActionResult GetVideos()
    {
        var videos = listVideo.GetVideos();
        var videosWithoutThumbnails = videos.Where(video => !System.IO.File.Exists(video.Thumbnail.Filepath)).ToList();
        thumbnailGenerator.GenerateThumbnailsAsync(videosWithoutThumbnails);
        return Ok(videos);
    }

    [HttpGet("{fileName}")]
    public IActionResult GetVideo(string fileName)
    {
        Console.Out.WriteLine("Lancement de la vidéo : " + fileName);
        var filePath = Path.Combine(_videoDirectory, fileName);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return new FileStreamResult(stream, "video/mp4");
    }
    
    private async void GenerateThumbnail(string videoFilePath, string thumbnailPath)
    {
        Console.WriteLine($"Génération de la vignette pour la vidéo : {videoFilePath}");
        
        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i \"{videoFilePath}\"")
            .AddParameter("-ss 00:00:05.000")
            .AddParameter("-vframes 1")
            .SetOutput(thumbnailPath);

        await conversion.Start();
        
        Console.WriteLine($"Fin de génération de la vignette pour la vidéo : {videoFilePath}");
    }
}