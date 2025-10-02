using GameOfLife.Models;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Persistence;

public class BoardRepository : IBoardRepository
{
    private readonly GameDbContext _context;

    public BoardRepository(GameDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(BoardEntity board)
    {
        await _context.Boards.AddAsync(board);
        await _context.SaveChangesAsync();
    }

    public async Task<BoardEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Boards.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task UpdateAsync(BoardEntity board)
    {
        _context.Boards.Update(board);
        await _context.SaveChangesAsync();
    }
}