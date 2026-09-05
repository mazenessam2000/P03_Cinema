using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public class Movie
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Movie name is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Release date is required.")]
    [Display(Name = "Release Date")]
    public DateTime ReleaseDate { get; set; }

    [StringLength(250, ErrorMessage = "Image URL cannot exceed 250 characters.")]
    [Display(Name = "Main Image")]
    public string? MainImage { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
    public string Status { get; set; } = string.Empty;

    [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes.")]
    public int DurationMinutes { get; set; }

    public ICollection<MovieImage> Images { get; set; } = [];
    public ICollection<MovieActor> MovieActors { get; set; } = [];
    public ICollection<MovieCategory> MovieCategories { get; set; } = [];
    public ICollection<ShowTime> ShowTimes { get; set; } = [];
}