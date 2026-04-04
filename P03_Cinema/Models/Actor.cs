namespace P03_Cinema.Models;

public class Actor
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public ICollection<MovieActor> MovieActors { get; set; } = [];
}