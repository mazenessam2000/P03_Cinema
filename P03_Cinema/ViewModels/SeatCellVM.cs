namespace P03_Cinema.ViewModels;

public class SeatCellVM
{
    public int ShowTimeSeatId { get; set; }
    public int SeatId { get; set; }

    public int? CartItemId { get; set; } // ✅ FIX

    public string Label { get; set; } = null!;
    public SeatType Type { get; set; }
    public SeatStatus Status { get; set; }

    public bool IsInMyCart { get; set; }

    public bool IsReservedByMe { get; set; } // ✅ NEW
}