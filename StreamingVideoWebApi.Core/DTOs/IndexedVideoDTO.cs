namespace StreamingVideoWebApi.Core.DTOs;

public class IndexedVideoDTO
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public TimeSpan Duration { get; set; }
}
