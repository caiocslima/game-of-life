import * as React from 'react';
import type { UseGameOfLifeReturn } from '@hooks';
import { Button } from '@components';
import { NUM_COLS, NUM_ROWS } from '@store';
import { NumberInput } from '../../components/NumberInput.tsx';

type ControlsProps = UseGameOfLifeReturn;

export const Controls = ({ state, actions }: ControlsProps) => {
  const { isRunning, isLoading, speed } = state;
  const [advanceCount, setAdvanceCount] = React.useState(10);
  const [maxFinalAttempts, setMaxFinalAttempts] = React.useState(100);
  const isControlDisabled = isRunning || isLoading;

  const handleRandomize = () => {
    const newGrid = Array.from({ length: NUM_ROWS }, () =>
      Array.from({ length: NUM_COLS }, () => Math.random() > 0.75),
    );
    actions.handleReset(newGrid);
  };

  const handleClear = () => {
    const newGrid = Array.from({ length: NUM_ROWS }, () =>
      Array.from({ length: NUM_COLS }, () => false),
    );
    actions.handleReset(newGrid);
  };

  return (
    <div className="flex flex-wrap items-center justify-center gap-2 md:gap-4 mb-4 p-2 bg-gray-800 rounded-lg">
      <Button
        onClick={actions.handlePlayPause}
        disabled={isLoading}
        className={'bg-green-500 hover:bg-green-600 w-28'}
      >
        {isRunning ? 'Pause' : 'Play'}
      </Button>
      <Button
        onClick={actions.handleNextStep}
        disabled={isControlDisabled}
        className={'bg-blue-500 hover:bg-blue-600'}
      >
        Next
      </Button>
      <div className="flex items-center gap-2">
        <Button
          onClick={() => actions.handleAdvanceX(advanceCount)}
          disabled={isControlDisabled}
          rounded={'left'}
          className={'bg-indigo-500 hover:bg-indigo-600'}
        >
          Advance
        </Button>
        <NumberInput
          value={advanceCount}
          onChange={(e) => setAdvanceCount(parseInt(e.target.value, 10) || 1)}
          disabled={isControlDisabled}
          title={'Number of generations to advance'}
        />
        <Button
          onClick={() => actions.handleFindFinalState(maxFinalAttempts)}
          disabled={isControlDisabled}
          rounded={'left'}
          className={'bg-purple-500 hover:bg-purple-600'}
        >
          Find Final
        </Button>
        <NumberInput
          value={maxFinalAttempts}
          onChange={(e) =>
            setMaxFinalAttempts(parseInt(e.target.value, 10) || 1)
          }
          disabled={isControlDisabled}
          title={'Maximum attempts to reach final state'}
        />
      </div>
      <Button
        onClick={handleRandomize}
        disabled={isControlDisabled}
        className={'bg-yellow-500 hover:bg-yellow-600'}
      >
        Randomize
      </Button>
      <Button
        onClick={handleClear}
        disabled={isControlDisabled}
        className={'bg-red-500 hover:bg-red-600'}
      >
        Clear
      </Button>
      <div className="flex items-center gap-2 text-sm">
        <label htmlFor="speed">Speed:</label>
        <input
          id="speed"
          type="range"
          min="50"
          max="1000"
          step="50"
          value={1050 - speed}
          onChange={(e) =>
            actions.handleSpeedChange(1050 - parseInt(e.target.value, 10))
          }
          className="w-24"
        />
      </div>
    </div>
  );
};
