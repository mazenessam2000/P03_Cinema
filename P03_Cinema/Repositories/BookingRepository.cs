namespace P03_Cinema.Repositories;

public class BookingRepository(ApplicationDbContext context) : IBookingRepository
{
    private readonly ApplicationDbContext _context = context;

    public void Add(Booking booking) => _context.Bookings.Add(booking);
    public void AddSeat(BookingSeat bookingSeat) => _context.BookingSeats.Add(bookingSeat);
}