using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Services;

public class CinemaService(
    IRepository<Cinema> cinemaRepo,
    IRepository<Movie> movieRepo,
    IRepository<ShowTime> showTimeRepo,
    IFileService fileService,
    IUnitOfWork unitOfWork) : ICinemaService
{
    private readonly IRepository<Cinema> _cinemaRepo = cinemaRepo;
    private readonly IRepository<Movie> _movieRepo = movieRepo;
    private readonly IRepository<ShowTime> _showTimeRepo = showTimeRepo;
    private readonly IFileService _fileService = fileService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CinemaIndexVM> GetCinemasPageAsync(int page, int pageSize, string q = "", CancellationToken ct = default)
    {
        var query = _cinemaRepo.Get().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Name.Contains(q) || x.Location.Contains(q));

        int total = await query.CountAsync(ct);
        int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var cinemas = await query
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var showCounts = await _showTimeRepo.Get()
            .GroupBy(x => x.CinemaId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);

        return new CinemaIndexVM
        {
            Cinemas = cinemas,
            ShowTimeCounts = showCounts,
            CurrentPage = page,
            TotalPages = totalPages,
            SearchQuery = q
        };
    }


    public async Task<CinemaCreateVM> GetCreateVMAsync(CancellationToken ct)
    {
        var movies = await _movieRepo.Get()
            .AsNoTracking()
            .ToListAsync(ct);

        return new CinemaCreateVM
        {
            AllMovies = movies
        };
    }

    public async Task CreateAsync(CinemaCreateVM vm, CancellationToken ct)
    {
        if (vm.Img != null)
        {
            vm.Cinema.ImageUrl = await _fileService.SaveImageAsync(vm.Img, FileType.Cinema, ct);
        }

        await _cinemaRepo.AddAsync(vm.Cinema, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<CinemaUpdateVM?> GetUpdateVMAsync(int id, CancellationToken ct)
    {
        var cinema = await _cinemaRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (cinema == null)
            return null;

        var movies = await _movieRepo.Get()
            .AsNoTracking()
            .ToListAsync(ct);

        var selected = await _showTimeRepo.Get()
            .Where(x => x.CinemaId == id)
            .Select(x => x.MovieId)
            .ToListAsync(ct);

        return new CinemaUpdateVM
        {
            Cinema = cinema,
            AllMovies = movies,
            SelectedMovieIds = selected
        };
    }

    public async Task UpdateAsync(CinemaUpdateVM vm, CancellationToken ct)
    {
        var cinema = await _cinemaRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == vm.Cinema.Id, ct)
            ?? throw new KeyNotFoundException("Cinema not found");

        cinema.Name = vm.Cinema.Name;
        cinema.Location = vm.Cinema.Location;
        cinema.IsActive = vm.Cinema.IsActive;
        cinema.TotalSeats = vm.Cinema.TotalSeats;

        if (vm.Img != null)
        {
            var oldImage = cinema.ImageUrl;

            cinema.ImageUrl = await _fileService.SaveImageAsync(vm.Img, FileType.Cinema, ct);

            if (!string.IsNullOrEmpty(oldImage))
                _fileService.Delete(oldImage, FileType.Cinema);
        }

        _cinemaRepo.Update(cinema);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var cinema = await _cinemaRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Cinema {id} not found");

        var relations = await _showTimeRepo.Get()
            .Where(x => x.CinemaId == id)
            .ToListAsync(ct);

        _showTimeRepo.RemoveRange(relations);

        if (!string.IsNullOrEmpty(cinema.ImageUrl))
            _fileService.Delete(cinema.ImageUrl, FileType.Cinema);

        _cinemaRepo.Remove(cinema);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}