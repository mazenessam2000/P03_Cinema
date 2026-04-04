namespace P03_Cinema.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public ICollection<MovieCategory> MovieCategories { get; set; } = [];
}