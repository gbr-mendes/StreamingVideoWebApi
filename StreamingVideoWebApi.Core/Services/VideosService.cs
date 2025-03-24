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
    
    public VideosService(
        IIndexedFilesRepository indexedFilesRepository,
        IS3StorageService s3StorageService)
    {
        _indexedFilesRepository = indexedFilesRepository;
        _s3StorageService = s3StorageService;
    }

    public async Task<IEnumerable<IndexedVideoVO>> GetIndexedVideos()
    {
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
        return indexedVideos;
    }

    public async Task<Option<IndexedVideoVO>> GetIndexedVideo(Guid id)
    {
        var indexedFile = await _indexedFilesRepository.GetIndexedFile(id);
        if (!indexedFile.HasValue)
        {
            return Option.None<IndexedVideoVO>();
        }

        return indexedFile.Map(indexedFile => new IndexedVideoVO(
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
    }
}
