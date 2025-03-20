using Optional;

namespace StreamingVideoWebApi.Core.ValueObjects;

public class IndexedVideoVO
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Path { get; private set; }
    public Option<string> Description { get; private set; }
    public Option<string> ThumbnailUrl { get; private set; }
    public long Size { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public TimeSpan Duration { get; private set; }

    public IndexedVideoVO(
        Guid id,
        string name,
        string path,
        Option<string> description,
        Option<string> thumbnailUrl,
        long size,
        DateTime createdAt,
        TimeSpan duration)
    {
        Id = id;
        Name = name;
        Path = path;
        Description = description;
        ThumbnailUrl = thumbnailUrl;
        Size = size;
        CreatedAt = createdAt;
        Duration = duration;
    }
}
