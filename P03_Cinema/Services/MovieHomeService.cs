using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Services;

public class MovieHomeService : IMovieHomeService
{
    private readonly IRepository<Movie> _movieRepo;

    public MovieHomeService(IRepository<Movie> movieRepo)
    {
        _movieRepo = movieRepo;
    }

    // ================= POPULAR =================
    public async Task<List<MovieCardVM>> GetPopularMoviesAsync()
    {
        var movies = await _movieRepo.Get()
            .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
            .OrderByDescending(m => m.Id)
            .Take(4)
            .ToListAsync();

        return movies.Select(MapToCardVM).ToList();
    }

    // ================= TRENDING =================
    public async Task<List<MovieCardVM>> GetTrendingMoviesAsync()
    {
        var query = _movieRepo.Get()
            .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
            .OrderByDescending(m => m.Id);

        var movies = await query.Skip(4).Take(2).ToListAsync();

        if (!movies.Any())
        {
            movies = await query.Take(2).ToListAsync();
        }

        return movies.Select(MapToCardVM).ToList();
    }
    // ================= BY CATEGORY =================
    public async Task<List<MovieCardVM>> GetMoviesByCategoryAsync(int categoryId)
    {
        var movies = await _movieRepo.Get()
            .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
            .Where(m => m.MovieCategories.Any(mc => mc.CategoryId == categoryId))
            .ToListAsync();

        return movies.Select(MapToCardVM).ToList();
    }

    // ================= DETAILS (UPDATED) =================
    public async Task<MovieDetailsVM?> GetMovieDetailsAsync(int id)
    {
        var movie = await _movieRepo.Get()
            .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
            .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
            .Include(m => m.Images)              // ✅ FIX 1: gallery images
            .Include(m => m.ShowTimes)           // ✅ FIX 2: showtimes
                .ThenInclude(st => st.Hall)
                    .ThenInclude(h => h.Cinema)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
            return null;

        return new MovieDetailsVM
        {
            Id = movie.Id,
            Name = movie.Name,
            Description = movie.Description,
            MainImage = movie.MainImage,
            Price = movie.Price,
            DurationMinutes = movie.DurationMinutes,
            Status = movie.Status,
            ReleaseDate = movie.ReleaseDate,

            // ================= CATEGORIES =================
            Categories = movie.MovieCategories
                .Select(mc => mc.Category.Name)
                .ToList(),

            // ================= ACTORS =================
            Actors = movie.MovieActors
                .Select(a => new ActorVM
                {
                    Id = a.Actor.Id,
                    Name = a.Actor.FullName,
                    ImageUrl = a.Actor.ImageUrl
                }).ToList(),

            // ================= GALLERY IMAGES (NEW) =================
            Images = movie.Images != null
                ? movie.Images.Select(i => i.ImageUrl).ToList()
                : new List<string>(),

            // ================= SHOWTIMES (NEW) =================
            ShowTimes = movie.ShowTimes.Select(st => new ShowTimeVM
            {
                Id = st.Id,
                StartTime = st.StartTime,
                CinemaName = st.Hall.Cinema.Name,
                HallName = st.Hall.Name
            }).ToList()
        };
    }

    // ================= MAPPING =================
    private MovieCardVM MapToCardVM(Movie m)
    {
        return new MovieCardVM
        {
            Id = m.Id,
            Name = m.Name,
            MainImage = m.MainImage,
            Status = m.Status,
            Price = m.Price,
            DurationMinutes = m.DurationMinutes,
            Categories = m.MovieCategories
                .Select(x => x.Category.Name)
                .ToList()
        };
    }

    public async Task<List<MovieCardVM>> GetAllMoviesAsync()
    {
        var movies = await _movieRepo.Get()
            .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
            .AsNoTracking()
            .ToListAsync();

        return movies.Select(m => new MovieCardVM
        {
            Id = m.Id,
            Name = m.Name,
            MainImage = m.MainImage,
            Status = m.Status,
            Price = m.Price,
            DurationMinutes = m.DurationMinutes,
            Categories = m.MovieCategories
                .Select(x => x.Category.Name)
                .ToList()
        }).ToList();
    }
}