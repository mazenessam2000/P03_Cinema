namespace P03_Cinema.ViewModels;

public class ActorCreateVM
{
    public Actor Actor { get; set; } = new Actor();

    public IFormFile? Img { get; set; }
}