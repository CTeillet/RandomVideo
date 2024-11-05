using RandomVideo.Models;

namespace RandomVideo.Service;

public class VideoService(
    IListVideo listVideo,
    IVideoThumbnailGenerator thumbnailGenerator,
    IVideoDirectoryProvider videoDirectoryProvider,
    ILogger<VideoService> logger) : IVideoService
{ 
    public List<Video> GetVideoAndCreateThumbnails()
    {
        var videos = listVideo.GetVideos();
        var videosWithoutThumbnails = videos.Where(video => !File.Exists(video.Thumbnail.Filepath)).ToList();
        thumbnailGenerator.GenerateThumbnailsAsync(videosWithoutThumbnails);
        return videos;
    }

    public FileStream GetVideo(string fileName)
    {
        logger.LogInformation("Lancement de la vidéo : {filename}", fileName);
        var filePath = Path.Combine(videoDirectoryProvider.GetVideoDirectory(), fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Le fichier vidéo n'a pas été trouvé.", fileName);
        }

        return new FileStream(filePath, FileMode.Open, FileAccess.Read);

    }
}