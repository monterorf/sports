using AutoMapper;
using MediatR;
using Sports.Application.DTOs;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Players.Queries.GetAllPlayers;

public class GetAllPlayersQueryHandler : IRequestHandler<GetAllPlayersQuery, IEnumerable<PlayerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPlayersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PlayerDto>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
    {
        var players = await _unitOfWork.Players.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PlayerDto>>(players);
    }
}
