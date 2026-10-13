using MediatR;
using Sports.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sports.Application.Features.Players.Commands.UpdatePlayer
{
    public record UpdatePlayerCommand(
        Guid Id, 
        string FirstName, 
        string LastName,
        DateTime DateOfBirth,
        string Nationality, 
        int Height, 
        int Weight, 
        Position Position, 
        int JerseyNumber, 
        Guid TeamId, 
        string? Photo = null) : IRequest<bool>;

}
