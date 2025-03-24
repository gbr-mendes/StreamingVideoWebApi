using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Optional.Unsafe;
using StreamingVideoWebApi.Core.DTOs;
using StreamingVideoWebApi.Core.Interfaces.Services;
using StreamingVideoWebApi.Core.ValueObjects;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StreamingVideoWebApi.Controllers;

[ApiController]
public class IndexedVideosController : ControllerBase
{
    private readonly IVideosService _videosService;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly string _cacheKey = "IndexedVideos";

    public IndexedVideosController(IVideosService videosService, IMapper mapper, IDistributedCache cache)
    {
        _videosService = videosService;
        _mapper = mapper;
        _cache = cache;
    }

    [HttpGet("api/indexed-videos")]
    public async Task<ActionResult<IEnumerable<IndexedVideoDTO>>> GetIndexedVideos()
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(_cacheKey);

            if (cachedData is not null)
            {
                var deserializedData = JsonSerializer.Deserialize<IEnumerable<IndexedVideoDTO>>(cachedData);
                return deserializedData != null ? Ok(deserializedData) : Ok(new List<IndexedVideoDTO>());
            }

            var indexedVideos = await _videosService.GetIndexedVideos();
            var indexedVideosDTO = _mapper.Map<IEnumerable<IndexedVideoDTO>>(indexedVideos);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) // TODO: Move the expiration time to a configuration file
            };

            _cache.SetString(_cacheKey, JsonSerializer.Serialize(indexedVideosDTO), cacheOptions);
            return Ok(indexedVideosDTO);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("api/videos/{videoId}")]
    public async Task<IActionResult> StreamVideo(string videoId, long? start = 0, long? end = null)
    {
        var videoMaybe = await _videosService.GetIndexedVideo(Guid.Parse(videoId.Replace("-", "").Replace("_", "")));
        if (!videoMaybe.HasValue)
        {
            return NotFound();
        }
        
        var video = videoMaybe.ValueOrFailure();

        var fileLength = video.Size;
        end ??= fileLength - 1;

        if (start >= fileLength || end >= fileLength)
        {
            return BadRequest("Invalid range");
        }

        var fileStream = new FileStream(video.Path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(start.Value, SeekOrigin.Begin);

        var contentRange = new ContentRangeHeaderValue(start.Value, end.Value, fileLength);
        Response.Headers.Add("Content-Range", contentRange.ToString());
        Response.StatusCode = 206; // Partial Content

        return File(fileStream, "video/mp4", enableRangeProcessing: true);
    }
}
