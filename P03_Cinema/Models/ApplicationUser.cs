using Microsoft.AspNetCore.Identity;

namespace P03_Cinema.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DOB { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}