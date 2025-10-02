namespace GameOfLife.Models.DTOs;

public class BoardStateResponse
{
    public Guid? BoardId { get; set; }
    public bool[][]? State { get; set; }
    public int Generation { get; set; }
    public string Stability { get; set; } = "Unstable";
}