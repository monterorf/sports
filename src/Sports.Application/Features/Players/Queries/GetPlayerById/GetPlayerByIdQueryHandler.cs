using AutoMapper;
using MediatR;
using Sports.Application.DTOs;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Players.Queries.GetPlayerById
{
    public class GetPlayerByIdQueryHandler : IRequestHandler<GetPlayerByIdQuery, PlayerDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPlayerByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PlayerDto?> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            var player = await _unitOfWork.Players.GetByIdAsync(request.Id, cancellationToken);
            if (player == null) return null;
            return _mapper.Map<PlayerDto>(player);
        }
    }
}
