export type Grid = boolean[][];

export enum BoardStability {
  UNSTABLE = 'Unstable',
  STABLE = 'Stable',
  OSCILLATOR = 'Oscillator',
}

export interface BoardCreationResponse {
  boardId: string;
}

export interface BoardStateResponse {
  boardId: string;
  state: Grid;
  generation: number;
  stability: BoardStability;
}
