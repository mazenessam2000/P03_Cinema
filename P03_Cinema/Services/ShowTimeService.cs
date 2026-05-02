using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Services;

public class ShowTimeService : IShowTimeService
{
    private readonly IRepository<ShowTime> _showTimeRepo;
    private readonly IRepository<Cinema> _cinemaRepo;
    private readonly IRepository<Movie> _movieRepo;
    private readonly IRepository<Seat> _seatRepo;
    private readonly IRepository<ShowTimeSeat> _showTimeSeatRepo;
    private readonly IRepository<Hall> _hallRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ShowTimeService(
        IRepository<ShowTime> showTimeRepo,
        IRepository<Cinema> cinemaRepo,
        IRepository<Movie> movieRepo,
        IRepository<ShowTimeSeat> showTimeSeatRepo,
        IRepository<Seat> seatRepo,
        IRepository<Hall> hallRepo,
        IUnitOfWork unitOfWork)
    {
        _showTimeRepo = showTimeRepo;
        _cinemaRepo = cinemaRepo;
        _movieRepo = movieRepo;
        _showTimeSeatRepo = showTimeSeatRepo;
        _seatRepo = seatRepo;
        _hallRepo = hallRepo;
        _unitOfWork = unitOfWork;
    }

    // ================= INDEX =================
    public async Task<ShowTimeIndexVM> GetAllAsync(int page, int pageSize, string q = "", CancellationToken ct = default)
    {
        var query = _showTimeRepo.Get()
            .Include(x => x.Movie)
            .Include(x => x.Cinema)
            .Include(x => x.Hall)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x =>
                x.Movie.Name.Contains(q) ||
                x.Cinema.Name.Contains(q));
        }

        var total = await query.CountAsync(ct);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var data = await query
            .OrderByDescending(x => x.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new ShowTimeIndexVM
        {
            ShowTimes = data,
            CurrentPage = page,
            TotalPages = totalPages,
            SearchQuery = q
        };
    }

    // ================= CREATE =================
    public async Task<ShowTimeCreateVM> GetCreateVMAsync(CancellationToken ct = default)
    {
        return new ShowTimeCreateVM
        {
            Movies = await _movieRepo.Get().AsNoTracking().ToListAsync(ct),
            Cinemas = await _cinemaRepo.Get().AsNoTracking().ToListAsync(ct),
            Halls = await _hallRepo.Get().AsNoTracking().ToListAsync(ct)
        };
    }

    public async Task CreateAsync(ShowTimeCreateVM vm, CancellationToken ct = default)
    {
        var movie = await _movieRepo.GetByIdAsync(vm.MovieId, ct)
            ?? throw new KeyNotFoundException("Movie not found");

        var cinema = await _cinemaRepo.GetByIdAsync(vm.CinemaId, ct)
            ?? throw new KeyNotFoundException("Cinema not found");

        var newStart = vm.StartTime;
        var newEnd = vm.StartTime.AddMinutes(movie.DurationMinutes);

        if (vm.StartTime < DateTime.UtcNow)
            throw new InvalidOperationException("Cannot create a showtime in the past.");

        // duplicate exact match
        var exists = await _showTimeRepo.Get()
            .AnyAsync(x =>
                x.MovieId == vm.MovieId &&
                x.CinemaId == vm.CinemaId &&
                x.StartTime == vm.StartTime, ct);

        if (exists)
            throw new InvalidOperationException("This showtime already exists");

        // FIX: Include Movie so DurationMinutes is available in the overlap query
        var overlaps = await _showTimeRepo.Get()
            .Where(x => x.CinemaId == vm.CinemaId && x.HallId == vm.HallId)
            .Include(x => x.Movie)
            .AnyAsync(x =>
                newStart < x.StartTime.AddMinutes(x.Movie.DurationMinutes) &&
                newEnd > x.StartTime, ct);

        if (overlaps)
            throw new InvalidOperationException(
                "This showtime overlaps with another showtime in the same cinema");

        var hall = await _hallRepo.GetByIdAsync(vm.HallId, ct)
            ?? throw new KeyNotFoundException("Hall not found");

        var showTime = new ShowTime
        {
            MovieId = vm.MovieId,
            CinemaId = vm.CinemaId,
            HallId = vm.HallId,
            StartTime = vm.StartTime,
            AvailableSeats = cinema.TotalSeats
        };


        await _showTimeRepo.AddAsync(showTime, ct);

        var hallSeats = await _seatRepo.Get()
            .Where(s => s.HallId == vm.HallId)
            .ToListAsync(ct);

        var showTimeSeats = hallSeats.Select(s => new ShowTimeSeat
        {
            ShowTime = showTime,
            SeatId = s.Id
        });

        await _showTimeSeatRepo.AddRangeAsync(showTimeSeats, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ================= UPDATE =================
    public async Task<ShowTimeUpdateVM?> GetUpdateVMAsync(int id, CancellationToken ct)
    {
        var show = await _showTimeRepo.Get()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (show == null)
            return null;

        return new ShowTimeUpdateVM
        {
            Id = show.Id,
            MovieId = show.MovieId,
            CinemaId = show.CinemaId,
            HallId = show.HallId, // ✅ REQUIRED
            StartTime = show.StartTime,

            Movies = await _movieRepo.Get()
                .AsNoTracking()
                .ToListAsync(ct),

            Cinemas = await _cinemaRepo.Get()
                .AsNoTracking()
                .ToListAsync(ct),

            Halls = await _hallRepo.Get()
                .Where(h => h.CinemaId == show.CinemaId)
                .AsNoTracking()
                .ToListAsync(ct)
        };
    }

    public async Task UpdateAsync(ShowTimeUpdateVM vm, CancellationToken ct = default)
    {
        var show = await _showTimeRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == vm.Id, ct);

        if (show == null)
            throw new KeyNotFoundException("ShowTime not found");

        // Prevent duplicate exact showtime
        var exists = await _showTimeRepo.Get()
            .AnyAsync(x =>
                x.Id != vm.Id &&
                x.MovieId == vm.MovieId &&
                x.CinemaId == vm.CinemaId &&
                x.StartTime == vm.StartTime, ct);

        if (exists)
            throw new InvalidOperationException("This showtime already exists");

        // Validate movie
        var movie = await _movieRepo.GetByIdAsync(vm.MovieId, ct)
            ?? throw new KeyNotFoundException("Movie not found");

        // Validate cinema
        var cinema = await _cinemaRepo.GetByIdAsync(vm.CinemaId, ct)
            ?? throw new KeyNotFoundException("Cinema not found");

        // ✅ Validate hall (this fixes your FK error)
        var hall = await _hallRepo.Get()
            .FirstOrDefaultAsync(h => h.Id == vm.HallId && h.CinemaId == vm.CinemaId, ct)
            ?? throw new InvalidOperationException("Invalid hall for this cinema");

        var newStart = vm.StartTime;
        var newEnd = vm.StartTime.AddMinutes(movie.DurationMinutes);

        // ✅ Safer overlap check (avoid EF translation issues)
        var showtimes = await _showTimeRepo.Get()
            .Where(x => x.CinemaId == vm.CinemaId && x.Id != vm.Id)
            .Include(x => x.Movie)
            .ToListAsync(ct);

        var overlaps = showtimes.Any(x =>
        {
            var existingEnd = x.StartTime.AddMinutes(x.Movie.DurationMinutes);
            return newStart < existingEnd && newEnd > x.StartTime;
        });

        if (overlaps)
            throw new InvalidOperationException("This showtime overlaps with another showtime in the same cinema");

        // ✅ Update
        show.MovieId = vm.MovieId;
        show.CinemaId = vm.CinemaId;
        show.HallId = vm.HallId;
        show.StartTime = vm.StartTime;

        // ✅ FIX: use hall capacity, not cinema
        show.AvailableSeats = hall.TotalSeats;

        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ================= DELETE =================
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var show = await _showTimeRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("ShowTime not found");

        _showTimeRepo.Remove(show);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}