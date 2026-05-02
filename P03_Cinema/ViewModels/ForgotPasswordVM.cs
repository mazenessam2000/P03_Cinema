using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.ViewModels;

public class ForgotPasswordVM
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; }
}