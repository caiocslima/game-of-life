import type { BoardCreationResponse, BoardStateResponse, Grid } from '@types';

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/api/boards';

class ApiError extends Error {
  constructor(
    message: string,
    public status: number,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

/**
 * Centralized response handler to process API responses and errors consistently.
 * It parses the ProblemDetails JSON from our custom middleware.
 * @param {Response} response - The raw fetch response.
 * @returns {Promise} The parsed JSON body.
 * @throws {ApiError} If the response is not ok.
 */
async function handleResponse<T>(response: Response): Promise<T> {
  if (response.ok) {
    return (await response.json()) as Promise<T>;
  }

  let errorMessage = `Request failed with status ${response.status}`;
  try {
    const errorBody = await response.json();
    errorMessage = errorBody.detail || errorMessage;
  } catch {
    // The body was not valid JSON, so we proceed with the generic status error.
  }

  throw new ApiError(errorMessage, response.status);
}

export const apiService = {
  /**
   * Sends a grid state to the API to create a new board in the database.
   * @param {Grid} grid - The initial state of the grid.
   * @returns {Promise<BoardCreationResponse>} The ID of the newly created board.
   */
  createBoard: async (grid: Grid): Promise<BoardCreationResponse> => {
    const response = await fetch(API_BASE_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ initialState: grid }),
    });
    return handleResponse<BoardCreationResponse>(response);
  },

  /**
   * Asks the API to calculate the state of a board after N generations.
   * @param {string} boardId - The ID of the board.
   * @param {number} generations - The target generation number.
   * @returns {Promise<BoardStateResponse>} The Nth state of the grid.
   */
  getNthState: async (
    boardId: string,
    generations: number,
  ): Promise<BoardStateResponse> => {
    const url = new URL(`${API_BASE_URL}/${boardId}/advance`);
    url.searchParams.append('generations', generations.toString());
    const response = await fetch(url.toString());
    return handleResponse<BoardStateResponse>(response);
  },

  /**
   * Asks the API to calculate the next state of a board.
   * @param {string} boardId - The ID of the board.
   * @returns {Promise<BoardStateResponse>} The next state of the grid.
   */
  getNextState: async (boardId: string): Promise<BoardStateResponse> => {
    const url = new URL(`${API_BASE_URL}/${boardId}/next`);
    const response = await fetch(url.toString());
    return handleResponse<BoardStateResponse>(response);
  },

  /**
   * Asks the API to find the final state (stable or oscillator) of a board.
   * @param {string} boardId - The ID of the board.
   * @param {number} [maxAttempts] - Optional max attempts to find the final state.
   * @returns {Promise<BoardStateResponse>} The final state of the grid.
   */
  getFinalState: async (
    boardId: string,
    maxAttempts?: number,
  ): Promise<BoardStateResponse> => {
    const url = new URL(`${API_BASE_URL}/${boardId}/final`);
    if (maxAttempts) {
      url.searchParams.append('maxAttempts', maxAttempts.toString());
    }
    const response = await fetch(url.toString());
    return handleResponse<BoardStateResponse>(response);
  },

  /**
   * Function to build the stream URL.
   * @param boardId The ID of the board to stream.
   * @param speed The delay between generations in milliseconds.
   * @param startGeneration The generation to start the stream from.
   * @returns The fully constructed URL for the EventSource.
   */
  getStreamUrl: (
    boardId: string,
    speed: number,
    startGeneration: number,
  ): string => {
    const url = new URL(`${API_BASE_URL}/${boardId}/stream`);
    url.searchParams.append('speed', speed.toString());
    url.searchParams.append('startGeneration', startGeneration.toString());
    return url.toString();
  },
};
