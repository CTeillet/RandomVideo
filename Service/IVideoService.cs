using RandomVideo.Models;

namespace RandomVideo.Service;

public interface IVideoService
{
    List<Video> GetVideoAndCreateThumbnails();
    FileStream GetVideo(string fileName);
}