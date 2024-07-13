namespace RandomVideo.Models;

public class Video(string name, string thumbnail)
{
    public string Name { get; set; } = name;
    public string Thumbnail { get; set; } = thumbnail;
}