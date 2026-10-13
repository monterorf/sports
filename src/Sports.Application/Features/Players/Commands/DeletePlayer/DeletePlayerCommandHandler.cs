using MediatR;
using Sports.Domain.Interfaces;
namespace Sports.Application.Features.Players.Commands.DeletePlayer;

public class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeletePlayerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.Players.ExistsAsync(request.Id, cancellationToken);
        if (!exists) return false;

        await _unitOfWork.Players.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
