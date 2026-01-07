using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sports.Application.Features.Teams.Commands.CreateTeam;
using Sports.Application.Features.Teams.Queries.GetAllTeams;
using Sports.Application.Features.Teams.Queries.GetTeamById;

namespace Sports.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllTeamsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTeamByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeamCommand command, CancellationToken cancellationToken)
    {
        var teamId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = teamId }, teamId);
    }
}
