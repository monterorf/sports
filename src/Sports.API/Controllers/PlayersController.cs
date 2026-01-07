using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sports.Application.Features.Players.Commands.CreatePlayer;
using Sports.Application.Features.Players.Queries.GetAllPlayers;

namespace Sports.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlayersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllPlayersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlayerCommand command, CancellationToken cancellationToken)
    {
        var playerId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = playerId }, playerId);
    }
}
