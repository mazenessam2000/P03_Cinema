using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.ViewModels;

public class VerifyPasswordVM
{
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}