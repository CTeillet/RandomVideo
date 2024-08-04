using RandomVideo.Models;

namespace RandomVideo.Service;

public class ListVideo(string videoDirectory, string thumbnailDirectory, ILogger<ListVideo> logger)
    : IListVideo
{
    private readonly HashSet<string> _videoExtensions =
        [".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".mpeg", ".mpg", ".m4v"];

    public List<Video> GetVideos()
    {
        logger.LogInformation("Récupération de la liste des vidéos");

        var videos = Directory.EnumerateFiles(videoDirectory, "*.*", SearchOption.AllDirectories)
            .Where(file => _videoExtensions.Contains(Path.GetExtension(file).ToLower()))
            .Select(s => Path.GetRelativePath(videoDirectory, s))
            .Select(video =>
                new Video(video,
                    new Thumbnail(Path.Combine(thumbnailDirectory, $"{Path.GetFileNameWithoutExtension(video)}.webp")),
                    Path.Combine(videoDirectory, video)))
            .ToList();

        return videos;
    }
}