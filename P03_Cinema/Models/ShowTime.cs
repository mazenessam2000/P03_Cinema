using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public class ShowTime
{
    public int Id { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public int HallId { get; set; }
    public Hall Hall { get; set; } = null!;

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public int CinemaId { get; set; }
    public Cinema Cinema { get; set; } = null!;

    [Range(1, 1000)]
    public int AvailableSeats { get; set; }

    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<ShowTimeSeat> ShowTimeSeats { get; set; } = [];
}