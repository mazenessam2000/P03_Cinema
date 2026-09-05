namespace P03_Cinema.ViewModels;

public class HallIndexVM
{
    public List<HallRowVM> Halls { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string Query { get; set; } = string.Empty;
}
