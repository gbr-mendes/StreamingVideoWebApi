using Optional;
using StreamingVideoWebApi.Core.ValueObjects;

namespace StreamingVideoWebApi.Core.Interfaces.Services;

public interface IVideosService
{
    Task<IEnumerable<IndexedVideoVO>> GetIndexedVideos();
    Task<Option<IndexedVideoVO>> GetIndexedVideo(Guid id);
}
