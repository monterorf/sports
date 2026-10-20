using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Sports.Application.Features.Teams.Commands.CreateTeam;
using Sports.Application.Features.Teams.Commands.UpdateTeam;
using Sports.Application.Features.Teams.Commands.DeleteTeam;
using Sports.Application.Features.Teams.Queries.GetAllTeams;
using Sports.Application.Features.Teams.Queries.GetTeamById;
using Sports.Domain.Constants;

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

    /// <summary>
    /// Get all teams (public endpoint - no authorization required)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllTeamsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get team by ID (public endpoint - no authorization required)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTeamByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Create a new team (Admin or LeagueManager only)
    /// </summary>
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.LeagueManager}")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeamCommand command, CancellationToken cancellationToken)
    {
        var teamId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = teamId }, teamId);
    }

    /// <summary>
    /// Update an existing team (Admin or TeamManager only)
    /// </summary>
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.TeamManager}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeamCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Delete a team (Admin only)
    /// </summary>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTeamCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Proxy external image (public endpoint - no authorization required)
    /// </summary>
    [HttpGet("proxy-image")]
    public async Task<IActionResult> ProxyImage([FromQuery] string url)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/png";
                return File(content, contentType);
            }
        }
        catch
        {
            return NotFound();
        }
    }
}
