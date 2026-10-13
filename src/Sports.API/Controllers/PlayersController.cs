using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sports.Application.Features.Players.Commands.CreatePlayer;
using Sports.Application.Features.Players.Commands.DeletePlayer;
using Sports.Application.Features.Players.Commands.UpdatePlayer;
using Sports.Application.Features.Players.Queries.GetAllPlayers;
using Sports.Application.Features.Players.Queries.GetPlayerById;

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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPlayerByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlayerCommand command, CancellationToken cancellationToken)
    {
        var playerId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = playerId }, playerId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlayerCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePlayerCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }


}
