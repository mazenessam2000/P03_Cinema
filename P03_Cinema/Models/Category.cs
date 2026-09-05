using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "Image URL cannot exceed 250 characters.")]
    [Display(Name = "Image")]
    public string ImageUrl { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; }

    public ICollection<MovieCategory> MovieCategories { get; set; } = [];
}