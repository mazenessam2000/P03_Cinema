using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public class Cinema
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Cinema name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
    public string Name { get; set; } = null!;

    [StringLength(250, ErrorMessage = "Location cannot exceed 250 characters.")]
    public string Location { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "Image URL cannot exceed 250 characters.")]
    [Display(Name = "Image")]
    public string ImageUrl { get; set; } = string.Empty;

    [Range(0.0, 10.0, ErrorMessage = "Rate must be between 0 and 10.")]
    public double Rate { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }

    [Range(1, 10000, ErrorMessage = "Total seats must be between 1 and 10000.")]
    [Display(Name = "Total Seats")]
    public int TotalSeats { get; set; }

    public ICollection<ShowTime> ShowTimes { get; set; } = [];
}