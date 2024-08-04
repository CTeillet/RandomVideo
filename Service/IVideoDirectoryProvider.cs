namespace RandomVideo.Service;

public interface IVideoDirectoryProvider
{
    string GetVideoDirectory();
    void SetVideoDirectory(string videoDirectory);
}