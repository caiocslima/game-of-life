using GameOfLife.Models;

namespace GameOfLife.Persistence;

public interface IBoardRepository
{
    Task<BoardEntity?> GetByIdAsync(Guid id);
    Task CreateAsync(BoardEntity board);
    Task UpdateAsync(BoardEntity board);
}