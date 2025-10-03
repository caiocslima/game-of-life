using System.Text.Json;
using System.Text.Json.Serialization;
using GameOfLife.Models.DTOs;
using GameOfLife.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Controllers;

[ApiController]
[Route("api/boards")]
public class GameOfLifeController : ControllerBase
{
    private readonly IGameOfLifeService _gameOfLifeService;

    public GameOfLifeController(IGameOfLifeService gameOfLifeService)
    {
        _gameOfLifeService = gameOfLifeService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest request)
    {
        var boardId = await _gameOfLifeService.CreateBoardAsync(request.InitialState);
        return CreatedAtAction(nameof(GetBoardState), new { id = boardId }, new BoardCreationResponse { BoardId = boardId });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBoardState(Guid id)
    {
        var board = await _gameOfLifeService.GetBoardByIdAsync(id);
        return Ok(board.ToStateResponse());
    }

    [HttpGet("{id:guid}/advance")]
    public async Task<IActionResult> GetNthState(Guid id, [FromQuery] int generations = 1)
    {
        var board = await _gameOfLifeService.GetNthStateAsync(id, generations);
        return Ok(board.ToStateResponse());
    }
    
    [HttpGet("{id:guid}/next")]
    public async Task<IActionResult> GetNextState(Guid id)
    {
        var board = await _gameOfLifeService.GetNextStateAsync(id);
        return Ok(board.ToStateResponse());
    }

    [HttpGet("{id:guid}/final")]
    public async Task<IActionResult> GetFinalState(Guid id, [FromQuery] int maxAttempts = 100)
    {
        var board = await _gameOfLifeService.GetFinalStateAsync(id, maxAttempts);
        return Ok(board.ToStateResponse());
    }

    [HttpGet("{id:guid}/stream")]
    public async Task StreamBoardUpdates(Guid id, [FromQuery] int speed = 200, int startGeneration = 0)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        try
        {
            await foreach (var boardState in _gameOfLifeService.StreamBoardUpdatesAsync(id, speed, startGeneration, HttpContext.RequestAborted))
            {
                var json = JsonSerializer.Serialize(boardState, options);
                await Response.WriteAsync($"data: {json}\n\n");
                await Response.Body.FlushAsync();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}