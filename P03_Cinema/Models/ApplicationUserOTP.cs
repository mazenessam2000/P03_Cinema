using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public class ApplicationUserOTP
{
    public int Id { get; set; }

    [Required]
    public string OTP { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpireDate { get; set; } = DateTime.UtcNow.AddMinutes(10);

    public bool IsUsed { get; set; } = false;

    public string ApplicationUserId { get; set; } = null!;
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public bool IsValid => DateTime.UtcNow < ExpireDate && !IsUsed;
}