namespace P03_Cinema.Models;

public enum SeatStatus { Available, Reserved, Booked }

public class ShowTimeSeat
{
    public int Id { get; set; }

    public int ShowTimeId { get; set; }
    public ShowTime ShowTime { get; set; } = null!;

    public int SeatId { get; set; }
    public Seat Seat { get; set; } = null!;

    public SeatStatus Status { get; set; } = SeatStatus.Available;

    public DateTime? ReservedUntil { get; set; }
    public string? ReservedByUserId { get; set; }
    public ApplicationUser? ReservedByUser { get; set; }

    public bool IsLockExpired =>
    ReservedUntil.HasValue && ReservedUntil.Value < DateTime.UtcNow;

    public ICollection<BookingSeat> BookingSeats { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];
}