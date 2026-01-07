using AutoMapper;
using MediatR;
using Sports.Application.DTOs;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Teams.Queries.GetAllTeams;

public class GetAllTeamsQueryHandler : IRequestHandler<GetAllTeamsQuery, IEnumerable<TeamDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTeamsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TeamDto>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
    {
        var teams = await _unitOfWork.Teams.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TeamDto>>(teams);
    }
}
