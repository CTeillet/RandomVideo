namespace RandomVideo.Service;

public class VideoDirectoryProvider : IVideoDirectoryProvider
{
    private string _videoDirectory;
    private readonly ILogger<VideoDirectoryProvider> _logger;

    public VideoDirectoryProvider(IConfiguration configuration, ILogger<VideoDirectoryProvider> logger)
    {
        _videoDirectory = configuration["PathVideoDirectory"];
        _logger = logger;
    }

    public string GetVideoDirectory()
    {
        return _videoDirectory;
    }

    public void SetVideoDirectory(string videoDirectory)
    {
        _logger.LogInformation("Changing video directory to: {VideoDirectory}", videoDirectory);
        _videoDirectory = videoDirectory;
    }
}