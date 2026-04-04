namespace P03_Cinema.ViewModels;

public class DashboardVM
{
    public int TicketsSold { get; set; }
    public decimal Revenue { get; set; }
    public int AvailableSeats { get; set; }
    public int ActiveMovies { get; set; }

    public List<ShowtimeVM> Showtimes { get; set; }
    public List<ActivityVM> Activities { get; set; }
    public List<string> Alerts { get; set; }
}
