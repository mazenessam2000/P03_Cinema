namespace P03_Cinema.ViewModels;

public class ShowtimeVM
{
    public string Movie { get; set; }
    public string Hall { get; set; }
    public string Time { get; set; }
    public int Progress { get; set; } // %
    public string Status { get; set; }
    public string StatusClass { get; set; }
}
