using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.ViewModels;

public class HallCreateVM
{
    [Required(ErrorMessage = "Hall name is required.")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Range(1, 100)]
    [Display(Name = "Total Rows")]
    public int TotalRows { get; set; }

    [Range(1, 100)]
    [Display(Name = "Seats Per Row")]
    public int SeatsPerRow { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Required]
    [Display(Name = "Cinema")]
    public int CinemaId { get; set; }

    // Populated by service
    public List<Cinema> Cinemas { get; set; } = [];
}
