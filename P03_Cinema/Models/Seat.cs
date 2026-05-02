using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public enum SeatType { Regular, VIP, Wheelchair }

public class Seat
{
    public int Id { get; set; }

    [Range(1, 100)]
    public int RowNumber { get; set; }

    [StringLength(5)]
    public string RowLabel { get; set; } = null!;

    [Range(1, 100)]
    public int SeatNumber { get; set; }

    public SeatType Type { get; set; } = SeatType.Regular;

    public bool IsActive { get; set; } = true;

    public int HallId { get; set; }
    public Hall Hall { get; set; } = null!;

    public ICollection<ShowTimeSeat> ShowTimeSeats { get; set; } = [];
}