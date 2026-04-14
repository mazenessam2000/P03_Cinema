namespace P03_Cinema.ViewModels;

public class CategoryUpdateVM
{
    public Category Category { get; set; } = new();

    public IFormFile? Img { get; set; }
}