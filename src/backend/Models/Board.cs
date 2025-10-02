using GameOfLife.Models.DTOs;

namespace GameOfLife.Models;

public enum BoardStability
{
    Unstable, // The board is still evolving.
    Stable,   // The board has reached a static, unchanging state.
    Oscillator// The board has entered a repeating cycle.
}

public class Board
    {
        public Guid Id { get; set; }
        public bool[][] State { get; private set; }
        public int Generation { get; private set; }
        public BoardStability Stability { get; private set; }

        private readonly int _rows;
        private readonly int _cols;
        private readonly HashSet<string> _history = new();

        public Board(Guid id, bool[][] initialState, int generation = 0, BoardStability stability = BoardStability.Unstable)
        {
            Id = id;
            State = initialState;
            Generation = generation;
            _rows = initialState.Length;
            _cols = initialState.Length > 0 ? initialState[0].Length : 0;
            Stability = stability;
        }

        /// <summary>
        /// Calculates the next generation and UPDATES the internal state of the board.
        /// </summary>
        public void CalculateNextState(bool verifyOscillator = false)
        {
            if (_rows == 0 || _cols == 0 || Stability != BoardStability.Unstable) return;

            var nextState = new bool[_rows][];
            for (var i = 0; i < _rows; i++)
            {
                nextState[i] = new bool[_cols];
                for (var j = 0; j < _cols; j++)
                {
                    var liveNeighbors = CountLiveNeighbors(i, j);
                    var isAlive = State[i][j];

                    if (isAlive && (liveNeighbors < 2 || liveNeighbors > 3))
                    {
                        nextState[i][j] = false; // Death by loneliness or overpopulation
                    }
                    else if (!isAlive && liveNeighbors == 3)
                    {
                        nextState[i][j] = true; // Birth
                    }
                    else
                    {
                        nextState[i][j] = isAlive; // Stays in the same state
                    }
                }
            }

            if (AreStatesIdentical(State, nextState))
            {
                Stability = BoardStability.Stable;
            }

            State = nextState;
            Generation++;

            if (verifyOscillator && Stability == BoardStability.Unstable)
            {
                var hash = GetStateHash();

                if (!_history.Add(hash))
                {
                    Stability = BoardStability.Oscillator;
                }
            }
        }

        public void AdvanceToGeneration(int targetGeneration)
        {
            while (Generation < targetGeneration && Stability == BoardStability.Unstable)
            {
                CalculateNextState();
            }
        }

        public BoardStability FindFinalState(int maxAttempts)
        {
            if (Stability != BoardStability.Unstable) return Stability;

            for (var i = 0; i < maxAttempts; i++)
            {
                CalculateNextState(true);
                if (Stability != BoardStability.Unstable) return Stability;
                
            }

            return BoardStability.Unstable;
        }

        public BoardStateResponse ToStateResponse()
        {
            return new BoardStateResponse
            {
                BoardId = Id,
                State = State,
                Generation = Generation,
                Stability = Stability.ToString()
            };
        }

        private int CountLiveNeighbors(int row, int col)
        {
            var count = 0;
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    var r = row + i;
                    var c = col + j;

                    if (r >= 0 && r < _rows && c >= 0 && c < _cols && State[r][c])
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public string GetStateHash() => string.Concat(State.Select(row => string.Concat(row.Select(c => c ? '1' : '0'))));

        private bool AreStatesIdentical(bool[][] stateA, bool[][] stateB)
        {
            for (var i = 0; i < _rows; i++)
            {
                for (var j = 0; j < _cols; j++)
                {
                    if (stateA[i][j] != stateB[i][j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }