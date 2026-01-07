using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sports.API.Controllers;
using Sports.Application.Features.Teams.Commands.CreateTeam;
using Xunit;

namespace Sports.API.Tests.Controllers;

public class TeamsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TeamsController _controller;

    public TeamsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new TeamsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = "Test Team",
            City = "Test City",
            FoundedYear = 2020
        };
        var expectedTeamId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mediatorMock
            .Setup(m => m.Send(command, cancellationToken))
            .ReturnsAsync(expectedTeamId);

        // Act
        var result = await _controller.Create(command, cancellationToken);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        
        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(TeamsController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(expectedTeamId);
        createdResult.Value.Should().Be(expectedTeamId);

        _mediatorMock.Verify(m => m.Send(command, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldCallMediator_WithCorrectCommand()
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = "Another Team",
            City = "Another City",
            FoundedYear = 2021
        };
        var expectedTeamId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTeamCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTeamId);

        // Act
        await _controller.Create(command, cancellationToken);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<CreateTeamCommand>(c => 
                    c.Name == command.Name && 
                    c.City == command.City && 
                    c.FoundedYear == command.FoundedYear),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldPassCancellationToken_ToMediator()
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = "Token Test Team",
            City = "Token City",
            FoundedYear = 2022
        };
        var expectedTeamId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTeamCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTeamId);

        // Act
        await _controller.Create(command, cancellationToken);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<CreateTeamCommand>(), cancellationToken),
            Times.Once);
    }
}
