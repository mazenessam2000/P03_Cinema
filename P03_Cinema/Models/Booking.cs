using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public class Booking
{
    public int Id { get; set; }

    public int ShowTimeId { get; set; }
    public ShowTime ShowTime { get; set; } = null!;

    [Range(1, 10)]
    public int SeatsCount { get; set; }

    [Range(0.01, 100000)]
    public decimal TotalPrice { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
}