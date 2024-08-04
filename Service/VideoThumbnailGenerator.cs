using System.Threading.Channels;
using RandomVideo.Models;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace RandomVideo.Service;

public class VideoThumbnailGenerator : IVideoThumbnailGenerator
{
    private readonly ILogger<VideoThumbnailGenerator> _logger;
    
    public VideoThumbnailGenerator(ILogger<VideoThumbnailGenerator> logger)
    {
        // Ensure the FFmpeg executables are downloaded
        var directoryWithFFmpegAndFFprobe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FFmpeg");
        FFmpeg.SetExecutablesPath(directoryWithFFmpegAndFFprobe);
        FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, directoryWithFFmpegAndFFprobe).Wait();
        _logger = logger;
    }

    public async Task GenerateThumbnailsAsync(List<Video> videos)
    {
        var channel = Channel.CreateBounded<Video>(new BoundedChannelOptions(10)
        {
            SingleWriter = false,
            SingleReader = false
        });

        var producer = Task.Run(async () =>
        {
            foreach (var video in videos.Where(video => !File.Exists(video.Thumbnail.Filepath)))
            {
                await channel.Writer.WriteAsync(video);
            }

            channel.Writer.Complete();
        });

        var consumers = Enumerable.Range(0, 5).Select(_ => Task.Run(async () =>
        {
            await foreach (var video in channel.Reader.ReadAllAsync())
            {
                await GenerateThumbnailAsync(video);
            }
        })).ToArray();

        await producer;
        await Task.WhenAll(consumers);
    }

    private async Task GenerateThumbnailAsync(Video video)
    {
        _logger.LogInformation("Génération de la vignette pour la vidéo : {videoName}, {videoPath}, {videoThumbnail}", video.Name, video.Path, video.Thumbnail);
        
        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i \"{video.Path}\"")
            .AddParameter("-ss 00:00:05.000")
            .AddParameter("-vframes 1")
            .SetOutput(video.Thumbnail.Filepath);

        await conversion.Start();
        
        _logger.LogInformation("Fin de génération de la vignette pour la vidéo : {videoName}", video.Name);
    }
}