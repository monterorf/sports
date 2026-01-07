using AutoMapper;
using MediatR;
using Sports.Application.DTOs;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Teams.Queries.GetTeamById;

public class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, TeamDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTeamByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TeamDto?> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(request.Id, cancellationToken);
        return team == null ? null : _mapper.Map<TeamDto>(team);
    }
}
