using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Services;

public class ShowTimeService : IShowTimeService
{
    private readonly IRepository<ShowTime> _showTimeRepo;
    private readonly IRepository<Cinema> _cinemaRepo;
    private readonly IRepository<Movie> _movieRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ShowTimeService(
        IRepository<ShowTime> showTimeRepo,
        IRepository<Cinema> cinemaRepo,
        IRepository<Movie> movieRepo,
        IUnitOfWork unitOfWork)
    {
        _showTimeRepo = showTimeRepo;
        _cinemaRepo = cinemaRepo;
        _movieRepo = movieRepo;
        _unitOfWork = unitOfWork;
    }

    // ================= INDEX =================
    public async Task<ShowTimeIndexVM> GetAllAsync(int page, int pageSize, string q = "", CancellationToken ct = default)
    {
        var query = _showTimeRepo.Get()
            .Include(x => x.Movie)
            .Include(x => x.Cinema)
            .AsQueryable();

        // 🔍 SEARCH (by movie or cinema)
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x =>
                x.Movie.Name.Contains(q) ||
                x.Cinema.Name.Contains(q));
        }

        int total = await query.CountAsync(ct);
        int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

        page = Math.Clamp(page, 1, totalPages);

        var data = await query
            .OrderByDescending(x => x.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
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
            Cinemas = await _cinemaRepo.Get().AsNoTracking().ToListAsync(ct)
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

        // duplicate exact match
        var exists = await _showTimeRepo.Get()
            .AnyAsync(x =>
                x.MovieId == vm.MovieId &&
                x.CinemaId == vm.CinemaId &&
                x.StartTime == vm.StartTime, ct);

        if (exists)
            throw new InvalidOperationException("This showtime already exists");

        // overlap check (FIXED)
        var overlaps = await _showTimeRepo.Get()
            .Where(x => x.CinemaId == vm.CinemaId)
            .AnyAsync(x =>
                newStart < x.StartTime.AddMinutes(x.Movie.DurationMinutes) &&
                newEnd > x.StartTime, ct);

        if (overlaps)
            throw new InvalidOperationException(
                "This showtime overlaps with another showtime in the same cinema");

        // create
        var showTime = new ShowTime
        {
            MovieId = vm.MovieId,
            CinemaId = vm.CinemaId,
            StartTime = vm.StartTime,
            AvailableSeats = cinema.TotalSeats
        };

        await _showTimeRepo.AddAsync(showTime, ct);
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
            StartTime = show.StartTime,
            Movies = await _movieRepo.Get().AsNoTracking().ToListAsync(ct),
            Cinemas = await _cinemaRepo.Get().AsNoTracking().ToListAsync(ct)
        };
    }

    public async Task UpdateAsync(ShowTimeUpdateVM vm, CancellationToken ct = default)
    {
        var show = await _showTimeRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == vm.Id, ct);

        if (show == null)
            throw new KeyNotFoundException("ShowTime not found");

        // 🔴 DUPLICATE CHECK (exclude itself)
        var exists = await _showTimeRepo.Get()
            .AnyAsync(x =>
                x.Id != vm.Id &&
                x.MovieId == vm.MovieId &&
                x.CinemaId == vm.CinemaId &&
                x.StartTime == vm.StartTime, ct);

        if (exists)
            throw new InvalidOperationException("This showtime already exists");

        // 🔴 GET MOVIE
        var movie = await _movieRepo.GetByIdAsync(vm.MovieId, ct)
            ?? throw new KeyNotFoundException("Movie not found");

        var newStart = vm.StartTime;
        var newEnd = vm.StartTime.AddMinutes(movie.DurationMinutes);

        // 🔴 OVERLAP CHECK (exclude itself)
        var overlaps = await _showTimeRepo.Get()
            .Where(x => x.CinemaId == vm.CinemaId && x.Id != vm.Id)
            .Include(x => x.Movie)
            .AnyAsync(x =>
                newStart < x.StartTime.AddMinutes(x.Movie.DurationMinutes) &&
                newEnd > x.StartTime, ct);

        if (overlaps)
            throw new InvalidOperationException("This showtime overlaps with another showtime in the same cinema");

        // 🔴 GET CINEMA
        var cinema = await _cinemaRepo.GetByIdAsync(vm.CinemaId, ct)
            ?? throw new KeyNotFoundException("Cinema not found");

        // 🔴 UPDATE
        show.MovieId = vm.MovieId;
        show.CinemaId = vm.CinemaId;
        show.StartTime = vm.StartTime;
        show.AvailableSeats = cinema.TotalSeats;

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