using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace GameOfLife.Models;

public class BoardEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Generation { get; set; }
    public DateTime CreatedAt { get; init; }
    public BoardStability Stability { get; set; } = BoardStability.Unstable;
    [NotMapped]
    public bool[][] CurrentState { get; set; } = Array.Empty<bool[]>();

    public string StateJson
    {
        get => JsonSerializer.Serialize(CurrentState);
        init => CurrentState = JsonSerializer.Deserialize<bool[][]>(value) ?? Array.Empty<bool[]>();
    }
}