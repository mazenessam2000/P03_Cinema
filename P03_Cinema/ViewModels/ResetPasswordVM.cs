namespace P03_Cinema.ViewModels;

using System.ComponentModel.DataAnnotations;

public class ResetPasswordVM
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}