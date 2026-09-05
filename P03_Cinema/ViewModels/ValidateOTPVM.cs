using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.ViewModels;

public class ValidateOTPVM
{
    public string UserId { get; set; }

    [Required(ErrorMessage = "Please enter the 6-digit code.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "The OTP must be exactly 6 digits.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Only numbers are allowed.")]
    public string OTP { get; set; }
}