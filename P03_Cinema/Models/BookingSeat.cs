namespace P03_Cinema.Models;

public class BookingSeat
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public int ShowTimeSeatId { get; set; }
    public ShowTimeSeat ShowTimeSeat { get; set; } = null!;

    public decimal PricePaid { get; set; }
}