using AutoMapper;
using Sports.Application.DTOs;
using Sports.Domain.Entities;

namespace Sports.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Team, TeamDto>()
            .ForMember(dest => dest.SportType, opt => opt.MapFrom(src => src.SportType.ToString()));

        CreateMap<Player, PlayerDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.GetAge()))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position.ToString()))
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null));

        CreateMap<Match, MatchDto>()
            .ForMember(dest => dest.HomeTeamName, opt => opt.MapFrom(src => src.HomeTeam != null ? src.HomeTeam.Name : string.Empty))
            .ForMember(dest => dest.AwayTeamName, opt => opt.MapFrom(src => src.AwayTeam != null ? src.AwayTeam.Name : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.SportType, opt => opt.MapFrom(src => src.SportType.ToString()))
            .ForMember(dest => dest.Winner, opt => opt.MapFrom(src => src.GetWinner()));

        CreateMap<MatchStatistic, MatchStatisticDto>()
            .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player != null ? src.Player.FullName : string.Empty))
            .ForMember(dest => dest.PassAccuracy, opt => opt.MapFrom(src => src.GetPassAccuracy()))
            .ForMember(dest => dest.ShotAccuracy, opt => opt.MapFrom(src => src.GetShotAccuracy()));
    }
}
