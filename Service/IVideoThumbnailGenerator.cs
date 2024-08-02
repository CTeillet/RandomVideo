using RandomVideo.Models;

namespace RandomVideo.Service;

public interface IVideoThumbnailGenerator
{
    Task GenerateThumbnailsAsync(List<Video> videos);
}