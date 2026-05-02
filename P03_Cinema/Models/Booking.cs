using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public class Booking
{
    public int Id { get; set; }

    public int ShowTimeId { get; set; }
    public ShowTime ShowTime { get; set; } = null!;

    [Range(0.01, 100000)]
    public decimal TotalPrice { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public ICollection<BookingSeat> BookingSeats { get; set; } = [];
    public int SeatsCount => BookingSeats.Count;

}