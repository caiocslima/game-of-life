import * as React from 'react';
import { apiService } from '@api';
import { BoardStability, type BoardStateResponse, type Grid } from '@types';
import { gameReducer, type GameState, initialState } from '@store';

interface UseGameOfLifeActions {
  handleCellClick: (row: number, col: number) => void;
  handlePlayPause: () => void;
  handleNextStep: () => void;
  handleAdvanceX: (steps: number) => void;
  handleFindFinalState: (maxAttempts: number) => void;
  handleReset: (newGrid: Grid) => void;
  handleSpeedChange: (speed: number) => void;
}

export interface UseGameOfLifeReturn {
  state: GameState;
  actions: UseGameOfLifeActions;
}

export const useGameOfLife = (): UseGameOfLifeReturn => {
  const [state, dispatch] = React.useReducer(gameReducer, initialState);

  const stateRef = React.useRef(state);
  stateRef.current = state;

  const ensureBoardId = React.useCallback(async (): Promise<string> => {
    if (stateRef.current.boardId) {
      return stateRef.current.boardId;
    }
    dispatch({ type: 'UPDATE_STATE', payload: { isLoading: true } });
    const { boardId: newBoardId } = await apiService.createBoard(
      stateRef.current.grid,
    );
    dispatch({
      type: 'UPDATE_STATE',
      payload: { boardId: newBoardId, isLoading: false },
    });
    return newBoardId;
  }, []);

  // Streaming
  React.useEffect(() => {
    if (!state.isRunning) {
      return;
    }

    let eventSource: EventSource | null = null;
    let isCancelled = false;

    const startStreaming = async () => {
      try {
        const currentBoardId = await ensureBoardId();

        if (isCancelled) return;

        const { speed, generation } = stateRef.current;
        const url = apiService.getStreamUrl(currentBoardId, speed, generation);
        eventSource = new EventSource(url);

        eventSource.onmessage = (event) => {
          const data: BoardStateResponse = JSON.parse(event.data);
          dispatch({
            type: 'UPDATE_STATE',
            payload: { grid: data.state, generation: data.generation },
          });

          if (data.stability !== BoardStability.UNSTABLE) {
            dispatch({ type: 'UPDATE_STATE', payload: { isRunning: false } });
          }
        };

        eventSource.onerror = () => {
          dispatch({
            type: 'UPDATE_STATE',
            payload: {
              errorMessage: 'Connection to the server was lost.',
              isRunning: false,
            },
          });
          eventSource?.close();
        };
      } catch {
        if (!isCancelled) {
          dispatch({
            type: 'UPDATE_STATE',
            payload: {
              errorMessage: 'Failed to start the simulation.',
              isRunning: false,
            },
          });
        }
      }
    };

    startStreaming().then();

    return () => {
      isCancelled = true;
      eventSource?.close();
    };
  }, [state.isRunning, state.speed, ensureBoardId]);

  const runApiCommand = React.useCallback(
    async (apiCall: (id: string) => Promise<BoardStateResponse>) => {
      if (stateRef.current.isRunning || stateRef.current.isLoading) return;
      dispatch({
        type: 'UPDATE_STATE',
        payload: { isLoading: true, errorMessage: null },
      });

      try {
        const currentBoardId = await ensureBoardId();
        const result = await apiCall(currentBoardId);
        dispatch({
          type: 'UPDATE_STATE',
          payload: {
            grid: result.state,
            generation: result.generation,
            stability: result.stability,
            isRunning:
              result.stability === BoardStability.UNSTABLE
                ? stateRef.current.isRunning
                : false,
          },
        });
      } catch (err: unknown) {
        const message =
          err instanceof Error ? err.message : 'An error occurred.';
        dispatch({
          type: 'UPDATE_STATE',
          payload: { errorMessage: message, boardId: null, isRunning: false },
        });
      } finally {
        dispatch({ type: 'UPDATE_STATE', payload: { isLoading: false } });
      }
    },
    [ensureBoardId],
  );

  // Actions
  const handleCellClick = React.useCallback((row: number, col: number) => {
    if (stateRef.current.isRunning || stateRef.current.isLoading) return;

    const newGrid = stateRef.current.grid.map((arr, r) =>
      r === row ? arr.map((cell, c) => (c === col ? !cell : cell)) : arr,
    );

    dispatch({ type: 'RESET_BOARD', payload: newGrid });
  }, []);

  const handlePlayPause = React.useCallback(() => {
    dispatch({
      type: 'UPDATE_STATE',
      payload: { isRunning: !stateRef.current.isRunning, errorMessage: null },
    });
  }, []);

  const handleReset = React.useCallback((newGrid: Grid) => {
    dispatch({ type: 'RESET_BOARD', payload: newGrid });
  }, []);

  const handleNextStep = React.useCallback(() => {
    runApiCommand((id) => apiService.getNextState(id)).then();
  }, [runApiCommand]);

  const handleAdvanceX = React.useCallback(
    (advanceCount: number) => {
      const targetGeneration = stateRef.current.generation + advanceCount;
      runApiCommand((id) =>
        apiService.getNthState(id, targetGeneration),
      ).then();
    },
    [runApiCommand],
  );

  const handleFindFinalState = React.useCallback(
    (maxAttempts: number) => {
      runApiCommand((id) => apiService.getFinalState(id, maxAttempts)).then();
    },
    [runApiCommand],
  );

  const handleSpeedChange = React.useCallback((newSpeed: number) => {
    dispatch({ type: 'UPDATE_STATE', payload: { speed: newSpeed } });
  }, []);

  return {
    state,
    actions: {
      handleCellClick,
      handlePlayPause,
      handleNextStep,
      handleAdvanceX,
      handleFindFinalState,
      handleReset,
      handleSpeedChange,
    },
  };
};
