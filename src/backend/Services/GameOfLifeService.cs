using System.Runtime.CompilerServices;
using GameOfLife.Middleware;
using GameOfLife.Models;
using GameOfLife.Models.DTOs;
using GameOfLife.Persistence;

namespace GameOfLife.Services;

public class GameOfLifeService : IGameOfLifeService
    {
        private readonly IBoardRepository _boardRepository;

        public GameOfLifeService(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        public async Task<Guid> CreateBoardAsync(bool[][] initialState)
        {
            var boardEntity = new BoardEntity
            {
                Id = Guid.NewGuid(),
                CurrentState = initialState,
                Generation = 0,
                CreatedAt = DateTime.UtcNow,
            };
            await _boardRepository.CreateAsync(boardEntity);
            return boardEntity.Id;
        }

        public async Task<Board> GetBoardByIdAsync(Guid id)
        {
            var boardEntity = await _boardRepository.GetByIdAsync(id) ?? throw new BoardNotFoundException($"Board with ID '{id}' not found.");
            return new Board(boardEntity.Id, boardEntity.CurrentState, boardEntity.Generation, boardEntity.Stability);
        }

        public async Task<Board> GetNextStateAsync(Guid id)
        {
            var board = await GetBoardByIdAsync(id);
            board.CalculateNextState();
            
            await UpdateBoard(board);

            return board;
        }

        public async Task<Board> GetNthStateAsync(Guid id, int generation)
        {
            var board = await GetBoardByIdAsync(id);
            board.AdvanceToGeneration(generation);
            
            await UpdateBoard(board);

            return board;
        }

        public async Task<Board> GetFinalStateAsync(Guid id, int maxAttempts)
        {
            var board = await GetBoardByIdAsync(id);
            var finalStability = board.FindFinalState(maxAttempts);

            if (finalStability == BoardStability.Unstable)
            {
                throw new NoFinalStateException($"Board did not reach a final state after {maxAttempts} attempts.");
            }

            await UpdateBoard(board);

            return board;
        }
        
        public async IAsyncEnumerable<BoardStateResponse> StreamBoardUpdatesAsync(Guid id, int speed, int startGeneration, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var board = await GetBoardByIdAsync(id);
            
            // Sync to the start generation requested by the client
            if (board.Generation < startGeneration)
            {
                board.AdvanceToGeneration(startGeneration);
            }

            // Loop for event stream
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Loop finished when stability is reached (oscillator or stable)
                    if (board.Stability != BoardStability.Unstable)
                    {
                        yield return board.ToStateResponse();
                        yield break;
                    }

                    yield return board.ToStateResponse();

                    await Task.Delay(speed, cancellationToken);

                    board.CalculateNextState(true);
                }
            }
            finally
            {
                await UpdateBoard(board);
            }
        }
        
        private async Task UpdateBoard(Board board)
        {
            var boardEntity = await _boardRepository.GetByIdAsync(board.Id);
            if (boardEntity != null)
            {
                boardEntity.CurrentState = board.State;
                boardEntity.Generation = board.Generation;
                boardEntity.Stability = board.Stability;
                await _boardRepository.UpdateAsync(boardEntity);
            }
        }
    }