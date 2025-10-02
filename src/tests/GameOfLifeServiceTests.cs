using GameOfLife.Middleware;
using GameOfLife.Models;
using GameOfLife.Persistence;
using GameOfLife.Services;
using Moq;
using Xunit;

namespace GameOfLife.Tests;

public class GameOfLifeServiceTests
{
    private readonly Mock<IBoardRepository> _mockRepo;
    private readonly GameOfLifeService _service;

    public GameOfLifeServiceTests()
    {
        _mockRepo = new Mock<IBoardRepository>();
        _service = new GameOfLifeService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateBoardAsync_CallsRepositoryCreateAsync()
    {
        // Arrange
        var initialState = new[] { new[] { true } };

        // Act
        await _service.CreateBoardAsync(initialState);

        // Assert
        _mockRepo.Verify(r => r.CreateAsync(It.Is<BoardEntity>(e => e.Stability == BoardStability.Unstable)), Times.Once);
    }

    [Fact]
    public async Task GetBoardByIdAsync_ThrowsBoardNotFoundException_WhenBoardDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(nonExistentId)).ReturnsAsync((BoardEntity?)null);
        
        // Act & Assert
        await Assert.ThrowsAsync<BoardNotFoundException>(() => _service.GetBoardByIdAsync(nonExistentId));
    }

    [Fact]
    public async Task GetFinalStateAsync_ShouldUpdateRepository_WhenBoardStabilizes()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var initialState = new[] // A blinker that will stabilize into an oscillator
        {
            new[] { false, false, false },
            new[] { true, true, true },
            new[] { false, false, false }
        };
        var entity = new BoardEntity { Id = boardId, CurrentState = initialState, Generation = 0, Stability = BoardStability.Unstable };
        _mockRepo.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetFinalStateAsync(boardId, 10);

        // Assert
        Assert.Equal(BoardStability.Oscillator, result.Stability);
        // Verify that the repository was called to save the new stable state
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<BoardEntity>(e => e.Stability == BoardStability.Oscillator)), Times.Once);
    }

    [Fact]
    public async Task GetFinalStateAsync_ShouldThrowNoFinalStateException_WhenBoardDoesNotStabilize()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var initialState = new[] // A glider that won't stabilize in 2 steps
        {
            new[] { false, true, false },
            new[] { false, false, true },
            new[] { true, true, true }
        };
        var entity = new BoardEntity { Id = boardId, CurrentState = initialState, Generation = 0, Stability = BoardStability.Unstable };
        _mockRepo.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync(entity);

        // Act & Assert
        await Assert.ThrowsAsync<NoFinalStateException>(() => _service.GetFinalStateAsync(boardId, 2));
    }
}
