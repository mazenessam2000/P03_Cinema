namespace P03_Cinema.ViewModels;

public class ActorUpdateVM
{
    public Actor Actor { get; set; } = new();

    public IFormFile? Img { get; set; }
}