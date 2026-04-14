using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public class ShowTime
{
    public int Id { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Range(1, 1000)]
    public int AvailableSeats { get; set; }

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public int CinemaId { get; set; }
    public Cinema Cinema { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = [];
}