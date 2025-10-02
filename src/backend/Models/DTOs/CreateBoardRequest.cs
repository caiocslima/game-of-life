using System.ComponentModel.DataAnnotations;

namespace GameOfLife.Models.DTOs;

public class CreateBoardRequest
{
    [Required]
    public bool[][] InitialState { get; set; } = null!;
}