namespace P03_Cinema.Interfaces.Repositories;

public interface IBookingRepository
{
    void Add(Booking booking);
    void AddSeat(BookingSeat bookingSeat);
}