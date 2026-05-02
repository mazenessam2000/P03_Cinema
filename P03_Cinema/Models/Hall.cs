using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public class Hall
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Range(1, 1000)]
    public int TotalRows { get; set; }

    [Range(1, 100)]
    public int SeatsPerRow { get; set; }

    public int TotalSeats => TotalRows * SeatsPerRow;

    public bool IsActive { get; set; } = true;

    public int CinemaId { get; set; }
    public Cinema Cinema { get; set; } = null!;

    public ICollection<Seat> Seats { get; set; } = [];
    public ICollection<ShowTime> ShowTimes { get; set; } = [];
}