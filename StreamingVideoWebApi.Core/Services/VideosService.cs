using Microsoft.Extensions.Caching.Distributed;
using Optional;
using StreamingVideoWebApi.Core.Interfaces.Repositories;
using StreamingVideoWebApi.Core.Interfaces.Services;
using StreamingVideoWebApi.Core.ValueObjects;
using System.Text.Json;

namespace StreamingVideoWebApi.Core.Services;

public class VideosService : IVideosService
{
    private readonly IIndexedFilesRepository _indexedFilesRepository;
    private readonly IS3StorageService _s3StorageService;
    private readonly IDistributedCache _cache;
    private readonly string _cacheKey = "indexedVideos";
    public VideosService(
        IIndexedFilesRepository indexedFilesRepository,
        IS3StorageService s3StorageService,
        IDistributedCache cache)
    {
        _indexedFilesRepository = indexedFilesRepository;
        _s3StorageService = s3StorageService;
        _cache = cache;
    }

    public async Task<IEnumerable<IndexedVideoVO>> GetIndexedVideos()
    {
        var cachedData = await _cache.GetStringAsync(_cacheKey);

        if (cachedData is not null)
        {
            var deserializedData = JsonSerializer.Deserialize<IEnumerable<IndexedVideoVO>>(cachedData);
            return deserializedData ?? [];
        }

        var indexedFiles = await _indexedFilesRepository.GetIndexedFiles();
        var indexedVideos = indexedFiles.Select(indexedFile => new IndexedVideoVO(
            id: indexedFile.Id,
            name: indexedFile.Name,
            path: indexedFile.Path,
            description: indexedFile.Description == null ? Option.None<string>() : Option.Some(indexedFile.Description),
            thumbnailUrl: indexedFile.ThumbnailPath == null ?
                Option.None<string>() :
                Option.Some(_s3StorageService.SignRemoteFileUrl(indexedFile.ThumbnailRemoteKey)),
            size: indexedFile.Size,
            createdAt: indexedFile.CreatedAt,
            duration: indexedFile.Duration
        ));

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) // TODO: Move the expiration time to a configuration file
        };

        _cache.SetString(_cacheKey, JsonSerializer.Serialize(indexedVideos), cacheOptions);

        return indexedVideos;
    }
}
