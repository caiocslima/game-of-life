using GameOfLife.Models;
using Xunit;

namespace GameOfLife.Tests;

public class BoardTests
{
    [Fact]
    public void CalculateNextState_DiesFromUnderpopulation()
    {
        // Arrange: Live cell with only 1 neighbor
        var initialState = new[]
        {
            new[] { false, false, false },
            new[] { false, true, true },
            new[] { false, false, false }
        };
        var board = new Board(Guid.NewGuid(), initialState);

        board.CalculateNextState();
        var finalState = board.State;

        // Assert: The middle cell must be disabled
        Assert.False(finalState[1][1]);
    }

    [Fact]
    public void CalculateNextState_SurvivesWithTwoNeighbors()
    {
        // Arrange: Live cell with 2 neighbors
        var initialState = new[]
        {
            new[] { false, true, false },
            new[] { false, true, false },
            new[] { false, true, false }
        };
        var board = new Board(Guid.NewGuid(), initialState);

        board.CalculateNextState();
        var finalState = board.State;

        // Assert: The middle cell must stay alive
        Assert.True(finalState[1][1]);
    }

    [Fact]
    public void CalculateNextState_DiesFromOverpopulation()
    {
        // Arrange: Live cell with 4 neighbors
        var initialState = new[]
        {
            new[] { true, true, true },
            new[] { true, true, false },
            new[] { false, false, false }
        };
        var board = new Board(Guid.NewGuid(), initialState);

        board.CalculateNextState();
        var finalState = board.State;

        // Assert: The middle cell must be disabled
        Assert.False(finalState[1][1]);
    }

    [Fact]
    public void CalculateNextState_BecomesAliveFromReproduction()
    {
        // Arrange: Dead cell with exactly 3 neighbors
        var initialState = new[]
        {
            new[] { false, true, false },
            new[] { true, false, true },
            new[] { false, false, false }
        };
        var board = new Board(Guid.NewGuid(), initialState);

        board.CalculateNextState();
        var finalState = board.State;

        // Assert: The middle cell must become alive
        Assert.True(finalState[1][1]);
    }

    [Fact]
    public void CalculateNextState_BlinkerPatternOscillatesCorrectly()
    {
        // Arrange: Blinker
        var initialState = new[]
        {
            new[] { false, false, false },
            new[] { true, true, true },
            new[] { false, false, false }
        };
        var board = new Board(Guid.NewGuid(), initialState);

        // First gen
        board.CalculateNextState();
        var stateAfterOneGen = board.State;

        // Second gen
        board.CalculateNextState();
        var stateAfterTwoGens = board.State;

        // Assert: After one gen, must be in vertical
        Assert.False(stateAfterOneGen[1][0]);
        Assert.True(stateAfterOneGen[1][1]);
        Assert.False(stateAfterOneGen[1][2]);
        Assert.True(stateAfterOneGen[0][1]);
        Assert.True(stateAfterOneGen[2][1]);

        // Assert: After two gens, goes back to initial state
        Assert.Equal(initialState, stateAfterTwoGens);
    }
    
    [Fact]
    public void FindFinalState_ShouldIdentifyStablePattern()
    {
        // Arrange: A 2x2 block is a classic "still life"
        var initialState = new[]
        {
            new[] { true, true },
            new[] { true, true }
        };
        var board = new Board(Guid.NewGuid(), initialState);

        // Act
        board.FindFinalState(10); // Should stabilize immediately

        // Assert
        Assert.Equal(BoardStability.Stable, board.Stability);
    }
    
    [Fact]
    public void FindFinalState_ShouldIdentifyOscillatorPattern()
    {
        // Arrange: A blinker pattern oscillates with a period of 2
        var initialState = new[]
        {
            new[] { false, false, false },
            new[] { true, true, true },
            new[] { false, false, false }
        };
        var board = new Board(Guid.NewGuid(), initialState);

        // Act
        board.FindFinalState(10);

        // Assert
        Assert.Equal(BoardStability.Oscillator, board.Stability);
    }

    [Fact]
    public void FindFinalState_ShouldRemainUnstable_WhenNoConclusionIsReached()
    {
        // Arrange: A "glider" pattern moves and won't stabilize in 3 steps
        var initialState = new[]
        {
            new[] { false, true, false, false },
            new[] { false, false, true, false },
            new[] { true, true, true, false },
            new[] { false, false, false, false }
        };
        var board = new Board(Guid.NewGuid(), initialState);

        // Act
        board.FindFinalState(3); // Not enough attempts to find a cycle

        // Assert
        Assert.Equal(BoardStability.Unstable, board.Stability);
    }
    
    [Fact]
    public void CalculateNextState_ShouldNotChangeState_WhenBoardIsStable()
    {
        // Arrange
        var initialState = new[]
        {
            new[] { true, true },
            new[] { true, true }
        };
        var board = new Board(Guid.NewGuid(), initialState);
        board.FindFinalState(1); // Force stability calculation
        
        var stateBefore = board.GetStateHash();

        // Act
        board.CalculateNextState(); // Attempt to advance a stable board
        var stateAfter = board.GetStateHash();

        // Assert
        Assert.Equal(BoardStability.Stable, board.Stability);
        Assert.Equal(stateBefore, stateAfter); // State should not have changed
    }
}
