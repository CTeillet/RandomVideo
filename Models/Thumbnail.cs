namespace RandomVideo.Models;

public class Thumbnail(string filepath)
{
    public string Filepath { get; set; } = filepath;
    public string FilepathDisplay { get; set; } = filepath.Substring(filepath.IndexOf('/') + 1);
}