using System.ComponentModel.DataAnnotations;
using P03_Cinema.Validations;

namespace P03_Cinema.Models;

public class Actor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 150 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = null!;

    [NotInFuture]
    [Display(Name = "Birth Date")]
    public DateTime? BirthDate { get; set; }

    [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
    public string Bio { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "Image URL cannot exceed 250 characters.")]
    [Display(Name = "Image")]
    public string ImageUrl { get; set; } = string.Empty;

    public ICollection<MovieActor> MovieActors { get; set; } = [];
}