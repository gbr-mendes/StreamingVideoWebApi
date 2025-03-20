using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StreamingVideoWebApi.Core.DTOs;
using StreamingVideoWebApi.Core.Interfaces.Services;

namespace StreamingVideoWebApi.Controllers;

[ApiController]
public class IndexedVideosController : ControllerBase
{
    private readonly IVideosService _videosService;
    private readonly IMapper _mapper;
    public IndexedVideosController(IVideosService videosService, IMapper mapper)
    {
        _videosService = videosService;
        _mapper = mapper;
    }

    [HttpGet("api/indexed-videos")]
    public async Task<ActionResult<IEnumerable<IndexedVideoDTO>>> GetIndexedVideos()
    {
        try
        {
            var indexedVideos = await _videosService.GetIndexedVideos();
            var indexedVideosDTO = _mapper.Map<IEnumerable<IndexedVideoDTO>>(indexedVideos);
            return Ok(indexedVideosDTO);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
