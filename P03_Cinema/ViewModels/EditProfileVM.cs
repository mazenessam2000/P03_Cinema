using System.ComponentModel.DataAnnotations;

public class EditProfileVM
{
    [MaxLength(50)]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [MaxLength(50)]
    public string? Username { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string? ConfirmNewPassword { get; set; }
}