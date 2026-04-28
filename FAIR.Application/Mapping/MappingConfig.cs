using AutoMapper;
using FAIR.Application.DTOs.Identity;
using FAIR.Application.DTOs.Profile;

using FAIR.Application.DTOs.Search;
using FAIR.Application.DTOs.Video;
using FAIR.Domain.Entities;
using FAIR.Domain.Entities.Identity;

namespace FAIR.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {


            CreateMap<Player, Register>().ReverseMap();
            CreateMap<Coach, Register>().ReverseMap();
            CreateMap<Player, UpdatePlayerProfile>().ReverseMap();
            CreateMap<Player, PlayerProfile>().ReverseMap();
            CreateMap<Coach, UpdateCoachProfile>().ReverseMap();
            CreateMap<Coach, CoachProfile>().ReverseMap();
            CreateMap<Player, AthleteSearchResult>()
                .ForMember(dest => dest.AthleteId, opt => opt.MapFrom(src => src.Id));

            CreateMap<AiModelMetricsDto, VideoAnalysis>()
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.OverallScore))
                .ForAllMembers(opt => opt.Condition((_, _, srcMember) => srcMember != null));

            CreateMap<AiModelResponseDto, VideoAnalysis>()
                .IncludeMembers(src => src.Metrics)
                .ForMember(dest => dest.AthleteId,
                    opt => opt.MapFrom((_, _, _, context) => (Guid)context.Items["AthleteId"]))
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.MapFrom((_, _, _, context) => (DateTime)context.Items["CreatedDate"]))
                .ForMember(dest => dest.Score,
                    opt => opt.MapFrom(src => src.Score ?? (src.Metrics != null ? src.Metrics.OverallScore : 0m)))
                .ForMember(dest => dest.AiResultRaw,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.RawJson)
                        ? (src.RawResult ?? src.Summary ?? string.Empty)
                        : src.RawJson));

            CreateMap<VideoAnalysis, VideoAnalysisResponseDto>()
                .ForMember(dest => dest.ProcessingStatus, opt => opt.MapFrom(src => src.ProcessingStatus.ToString()));
        }
    }
}
