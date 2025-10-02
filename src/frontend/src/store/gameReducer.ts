import { BoardStability, type Grid } from '@types';

export const NUM_ROWS = 30;
export const NUM_COLS = 50;

export interface GameState {
  grid: Grid;
  isRunning: boolean;
  isLoading: boolean;
  speed: number;
  generation: number;
  boardId: string | null;
  errorMessage: string | null;
  stability: BoardStability;
}

export type GameAction =
  | { type: 'UPDATE_STATE'; payload: Partial<GameState> }
  | { type: 'RESET_BOARD'; payload: Grid };

export const initialState: GameState = {
  grid: Array.from({ length: NUM_ROWS }, () =>
    Array.from({ length: NUM_COLS }, () => false),
  ),
  isRunning: false,
  isLoading: false,
  speed: 200,
  generation: 0,
  boardId: null,
  errorMessage: null,
  stability: BoardStability.UNSTABLE,
};

export const gameReducer = (
  state: GameState,
  action: GameAction,
): GameState => {
  switch (action.type) {
    case 'UPDATE_STATE':
      return { ...state, ...action.payload };
    case 'RESET_BOARD':
      return {
        ...state,
        grid: action.payload,
        generation: 0,
        boardId: null,
        errorMessage: null,
        isRunning: false,
        stability: BoardStability.UNSTABLE,
      };
    default:
      return state;
  }
};
