namespace RandomVideo.Models;

public class Video(string name, Thumbnail thumbnail, string path)
{
    public string Name { get; set; } = name;
    public Thumbnail Thumbnail { get; set; } = thumbnail;
    public string Path { get; set; } = path;
}