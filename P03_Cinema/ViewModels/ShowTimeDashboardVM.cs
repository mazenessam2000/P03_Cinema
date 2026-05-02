namespace P03_Cinema.ViewModels;
public class ShowTimeDashboardVM
{
    public int ShowTimeId { get; set; }

    public string MovieName { get; set; } = string.Empty;

    public string HallName { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    // Display-friendly time (optional helper)
    public string Time => StartTime.ToString("HH:mm");

    // UI: Running / Upcoming / Finished
    public string Status { get; set; } = string.Empty;

    // 0–100 progress for progress bar
    public int Progress { get; set; }

    // Optional stats
    public int TotalSeats { get; set; }

    public int BookedSeats { get; set; }

    public int AvailableSeats => TotalSeats - BookedSeats;

    public decimal Revenue { get; set; }
}