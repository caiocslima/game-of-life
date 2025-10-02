using GameOfLife.Models;
using GameOfLife.Models.DTOs;

namespace GameOfLife.Services;

public interface IGameOfLifeService
{
    Task<Guid> CreateBoardAsync(bool[][] initialState);
    Task<Board> GetBoardByIdAsync(Guid id);
    Task<Board> GetNextStateAsync(Guid id);
    Task<Board> GetNthStateAsync(Guid id, int generation);
    Task<Board> GetFinalStateAsync(Guid id, int maxAttempts);
    IAsyncEnumerable<BoardStateResponse> StreamBoardUpdatesAsync(Guid id, int speed, int startGeneration, CancellationToken cancellationToken);
}