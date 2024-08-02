using System.Threading.Channels;
using RandomVideo.Models;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace RandomVideo.Service;

public class VideoThumbnailGenerator : IVideoThumbnailGenerator
{
    public VideoThumbnailGenerator()
    {
        // Ensure the FFmpeg executables are downloaded
        var directoryWithFFmpegAndFFprobe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FFmpeg");
        FFmpeg.SetExecutablesPath(directoryWithFFmpegAndFFprobe);
        FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, directoryWithFFmpegAndFFprobe).Wait();
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
        Console.WriteLine($"Génération de la vignette pour la vidéo : {video.Name}, {video.Path}, {video.Thumbnail}");
        
        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i \"{video.Path}\"")
            .AddParameter("-ss 00:00:05.000")
            .AddParameter("-vframes 1")
            .SetOutput(video.Thumbnail.Filepath);

        await conversion.Start();
        
        Console.WriteLine($"Fin de génération de la vignette pour la vidéo : {video.Name}");
    }
}