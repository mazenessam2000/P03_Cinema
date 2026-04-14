namespace P03_Cinema.ViewModels;

public class MovieCreateVM
{
    public Movie Movie { get; set; } = new();

    public List<int>? SelectedActors { get; set; }
    public List<int>? SelectedCategories { get; set; }

    public IEnumerable<Actor> AllActors { get; set; } = new List<Actor>();
    public IEnumerable<Category> AllCategories { get; set; } = new List<Category>();

    public IFormFile? MainImg { get; set; }
    public List<IFormFile>? Gallery { get; set; }
}