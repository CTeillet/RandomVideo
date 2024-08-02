using RandomVideo.Models;

namespace RandomVideo.Service;

public interface IListVideo
{
    List<Video> GetVideos();
}