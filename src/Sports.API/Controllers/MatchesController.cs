using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sports.Application.Features.Matches.Queries.GetAllMatches;

namespace Sports.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllMatchesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
