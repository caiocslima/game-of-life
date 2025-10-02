import { useGameOfLife } from '@hooks';
import { Controls } from './Controls.tsx';
import { GridDisplay } from './GridDisplay.tsx';
import { BoardStability } from '@types';

export const GameOfLife = () => {
  const { state, actions } = useGameOfLife();

  const getStatusText = () => {
    const baseText = `Generation: ${state.generation}`;
    if (state.stability === BoardStability.STABLE) {
      return `${baseText} (Stable)`;
    }
    if (state.stability === BoardStability.OSCILLATOR) {
      return `${baseText} (Oscillator)`;
    }
    return baseText;
  };

  return (
    <div className="bg-gray-900 text-white min-h-screen flex flex-col items-center justify-center p-4 font-sans">
      <div className="w-full max-w-7xl">
        <h1 className="text-4xl font-bold text-center mb-2 text-green-400">
          Conway's Game of Life
        </h1>
        <p className="text-center text-gray-400 mb-4">{getStatusText()}</p>
        {state.errorMessage && (
          <p className="text-center text-red-500 mb-4 font-semibold">
            {state.errorMessage}
          </p>
        )}

        <Controls state={state} actions={actions} />
        <GridDisplay
          grid={state.grid}
          isLoading={state.isLoading}
          onCellClick={actions.handleCellClick}
        />
      </div>
    </div>
  );
};
