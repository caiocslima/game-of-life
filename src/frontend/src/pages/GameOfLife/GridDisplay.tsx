import { Cell } from './Cell.tsx';
import type { Grid } from '@types';
import { LoadingSpinner } from '@components';

interface GridDisplayProps {
  grid: Grid;
  isLoading: boolean;
  onCellClick: (row: number, col: number) => void;
}

const NUM_COLS = 50;
const NUM_ROWS = 30;

export const GridDisplay = ({
  grid,
  isLoading,
  onCellClick,
}: GridDisplayProps) => {
  return (
    <div
      className="relative border-t border-l border-gray-700 shadow-lg"
      style={{
        display: 'grid',
        gridTemplateColumns: `repeat(${NUM_COLS}, 1fr)`,
        aspectRatio: `${NUM_COLS} / ${NUM_ROWS}`,
      }}
    >
      {isLoading && <LoadingSpinner />}
      {grid.map((rows, i) =>
        rows.map((_, k) => (
          <Cell
            key={`${i}-${k}`}
            isAlive={grid[i][k]}
            onClick={() => onCellClick(i, k)}
          />
        )),
      )}
    </div>
  );
};
