namespace P03_Cinema.ViewModels;

public class AuthMessageVM
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string IconPath { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? RedirectAction { get; set; }
}