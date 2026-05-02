namespace P03_Cinema.ViewModels;

public class MovieCardVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? MainImage { get; set; }
    public string Status { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }

    public List<string> Categories { get; set; } = new();
}