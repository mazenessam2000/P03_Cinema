using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.ViewModels;

public class ShowTimeUpdateVM
{
    public int Id { get; set; }

    [Required]
    public int MovieId { get; set; }

    [Required]
    public int CinemaId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public List<Movie> Movies { get; set; } = [];
    public List<Cinema> Cinemas { get; set; } = [];
}