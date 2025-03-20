using AutoMapper;
using Optional.Unsafe;
using StreamingVideoWebApi.Core.DTOs;
using StreamingVideoWebApi.Core.ValueObjects;

namespace StreamingVideoWebApi.Infra.MappingProfiles;

public class IndexedVideoProfile : Profile
{
    public IndexedVideoProfile()
    {
        CreateMap<IndexedVideoVO, IndexedVideoDTO>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.HasValue ? src.Description.ValueOrFailure() : null))
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.ThumbnailUrl.HasValue ? src.ThumbnailUrl.ValueOrFailure() : null));
    }
}
