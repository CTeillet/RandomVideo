using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RandomVideo.Components.Pages;
using RandomVideo.Models;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace RandomVideo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly string videoDirectory = @"C:\Users\teill\Videos";
    private readonly string thumbnailDirectory = "wwwroot/thumbnails";

    public VideoController()
    {
        // Ensure the FFmpeg executables are downloaded
        var directoryWithFFmpegAndFFprobe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FFmpeg");
        FFmpeg.SetExecutablesPath(directoryWithFFmpegAndFFprobe);
        FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, directoryWithFFmpegAndFFprobe).Wait();
    }
    
    [HttpGet]
    public IActionResult GetVideos()
    {
        Console.Out.WriteLine("Récupération de la liste des vidéos");
        var videos = Directory.EnumerateFiles(videoDirectory, "*.mp4", SearchOption.AllDirectories)
            .Select(s => Path.GetRelativePath(videoDirectory, s))
            .ToList();
        
        var videoInfos = new List<Video>();

        foreach (var video in videos)
        {
            var videoInfo = new
            Video(video, $"/thumbnails/{Path.GetFileNameWithoutExtension(video)}.jpg");

            var thumbnailPath = Path.Combine(thumbnailDirectory, $"{Path.GetFileNameWithoutExtension(video)}.jpg");

            // Générer la vignette si elle n'existe pas
            if (!System.IO.File.Exists(thumbnailPath))
            {
                var videoFilePath = Path.Combine(videoDirectory, video);
                GenerateThumbnail(videoFilePath, thumbnailPath);
            }

            videoInfos.Add(videoInfo);
        }

        return Ok(videoInfos);
    }

    [HttpGet("{fileName}")]
    public IActionResult GetVideo(string fileName)
    {
        Console.Out.WriteLine("Lancement de la vidéo : " + fileName);
        var filePath = Path.Combine(videoDirectory, fileName);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return new FileStreamResult(stream, "video/mp4");
    }
    
    private async void GenerateThumbnail(string videoFilePath, string thumbnailPath)
    {
        Console.Out.WriteLine("Génération de la vignette pour la vidéo : " + videoFilePath);
        var mediaInfo = await FFmpeg.GetMediaInfo(videoFilePath);

        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i {videoFilePath}")
            .AddParameter("-ss 00:00:05.000")
            .AddParameter("-vframes 1")
            .SetOutput(thumbnailPath);

        await conversion.Start();
    }
}