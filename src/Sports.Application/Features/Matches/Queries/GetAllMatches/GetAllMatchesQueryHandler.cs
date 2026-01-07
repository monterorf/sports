using AutoMapper;
using MediatR;
using Sports.Application.DTOs;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Matches.Queries.GetAllMatches;

public class GetAllMatchesQueryHandler : IRequestHandler<GetAllMatchesQuery, IEnumerable<MatchDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMatchesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MatchDto>> Handle(GetAllMatchesQuery request, CancellationToken cancellationToken)
    {
        var matches = await _unitOfWork.Matches.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<MatchDto>>(matches);
    }
}
