import * as React from 'react';

interface CellProps {
  isAlive: boolean;
  onClick: () => void;
}

export const Cell: React.FC<CellProps> = React.memo(({ isAlive, onClick }) => {
  return (
    <div
      onClick={onClick}
      className={`w-full h-full border-r border-b border-gray-700 ${
        isAlive ? 'bg-green-400' : 'bg-gray-800'
      } transition-colors duration-200`}
    />
  );
});
