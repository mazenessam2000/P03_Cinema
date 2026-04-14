using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Services;

public class MovieService(IRepository<Movie> movieRepo,
    IRepository<MovieActor> movieActorsRepo,
    IRepository<MovieCategory> movieCategoriesRepo,
    IRepository<MovieImage> movieImagesRepo,
    IRepository<Actor> actorRepo,
    IRepository<Category> categoryRepo,
    IUnitOfWork unitOfWork,
    IFileService fileService) : IMovieService
{
    readonly IRepository<Movie> _movieRepo = movieRepo;
    private readonly IRepository<MovieActor> _movieActorsRepo = movieActorsRepo;
    private readonly IRepository<MovieCategory> _movieCategoriesRepo = movieCategoriesRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileService _fileService = fileService;
    private readonly IRepository<MovieImage> _movieImagesRepo = movieImagesRepo;
    private readonly IRepository<Actor> _actorRepo = actorRepo;
    private readonly IRepository<Category> _categoryRepo = categoryRepo;

    public async Task<MovieIndexVM> GetMoviesPageAsync(int page, int pageSize, string q = "", CancellationToken ct = default)
    {
        var query = _movieRepo.Get().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Name.Contains(q));

        int totalMovies = await query.CountAsync(ct);
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalMovies / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var movies = await query
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var movieIds = movies.Select(m => m.Id).ToList();

        var actors = await _movieActorsRepo.Get()
            .Where(x => movieIds.Contains(x.MovieId))
            .Include(x => x.Actor)
            .GroupBy(x => x.MovieId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(x => x.Actor.FullName).ToList(),
                ct
            );

        var categories = await _movieCategoriesRepo.Get()
            .Where(x => movieIds.Contains(x.MovieId))
            .Include(x => x.Category)
            .GroupBy(x => x.MovieId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(x => x.Category.Name).ToList(),
                ct
            );

        var images = await _movieRepo.Get()
            .Where(m => movieIds.Contains(m.Id))
            .Include(m => m.Images)
            .ToDictionaryAsync(
                m => m.Id,
                m =>
                {
                    var list = new List<string>();

                    if (!string.IsNullOrEmpty(m.MainImage))
                        list.Add(m.MainImage);

                    if (m.Images != null)
                        list.AddRange(m.Images.Select(i => i.ImageUrl));

                    return list;
                },
                ct
            );

        return new MovieIndexVM
        {
            Movies = movies,
            MovieActors = actors,
            MovieCategories = categories,
            MovieImages = images,
            CurrentPage = page,
            TotalPages = totalPages,
            SearchQuery = q
        };
    }

    public async Task<MovieCreateVM> GetCreateVmAsync(CancellationToken ct = default)
    {
        var vm = new MovieCreateVM
        {
            AllActors = await _actorRepo.Get()
                .AsNoTracking()
                .ToListAsync(ct),

            AllCategories = await _categoryRepo.Get()
                .AsNoTracking()
                .ToListAsync(ct)
        };

        return vm;
    }

    public async Task FillCreateVmAsync(MovieCreateVM vm, CancellationToken ct)
    {
        vm.AllActors = await _actorRepo.Get()
            .AsNoTracking()
            .ToListAsync(ct);

        vm.AllCategories = await _categoryRepo.Get()
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task CreateAsync(MovieCreateVM vm, CancellationToken ct = default)
    {
        if (vm.MainImg != null)
        {
            vm.Movie.MainImage = await _fileService.SaveImageAsync(vm.MainImg, FileType.Movie, ct);
        }

        vm.Movie.MovieActors = vm.SelectedActors?
            .Select(id => new MovieActor { ActorId = id })
            .ToList() ?? new();

        vm.Movie.MovieCategories = vm.SelectedCategories?
            .Select(id => new MovieCategory { CategoryId = id })
            .ToList() ?? new();

        vm.Movie.Images = new List<MovieImage>();

        if (vm.Gallery?.Any(f => f.Length > 0) == true)
        {
            foreach (var img in vm.Gallery)
            {
                var fileName = await _fileService.SaveImageAsync(img, FileType.Movie, ct);

                vm.Movie.Images.Add(new MovieImage
                {
                    ImageUrl = fileName
                });
            }
        }

        await _movieRepo.AddAsync(vm.Movie, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<MovieUpdateVM?> GetUpdateVMAsync(int id, CancellationToken ct)
    {
        var movie = await _movieRepo.Get()
            .Include(x => x.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (movie == null)
            throw new KeyNotFoundException("Movie not found");

        var vm = new MovieUpdateVM
        {
            Movie = movie,

            AllActors = await _actorRepo.Get()
            .AsNoTracking()
            .ToListAsync(ct),

            AllCategories = await _categoryRepo.Get()
            .AsNoTracking()
            .ToListAsync(ct),

            SelectedActors = await _movieActorsRepo.Get()
                .Where(x => x.MovieId == id)
                .Select(x => x.ActorId)
                .ToListAsync(ct),

            SelectedCategories = await _movieCategoriesRepo.Get()
                .Where(x => x.MovieId == id)
                .Select(x => x.CategoryId)
                .ToListAsync(ct)
        };

        return vm;
    }

    public async Task FillUpdateVMAsync(MovieUpdateVM vm, CancellationToken ct = default)
    {
        vm.AllActors = await _actorRepo.Get()
            .AsNoTracking()
            .ToListAsync(ct);

        vm.AllCategories = await _categoryRepo.Get()
            .AsNoTracking()
            .ToListAsync(ct);

        vm.SelectedActors ??= await _movieActorsRepo.Get()
            .Where(x => x.MovieId == vm.Movie.Id)
            .Select(x => x.ActorId)
            .ToListAsync(ct);

        vm.SelectedCategories ??= await _movieCategoriesRepo.Get()
            .Where(x => x.MovieId == vm.Movie.Id)
            .Select(x => x.CategoryId)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(MovieUpdateVM vm, CancellationToken ct = default)
    {
        var movie = await _movieRepo.Get()
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == vm.Movie.Id, ct);

        if (movie == null)
            throw new KeyNotFoundException("Movie not found");

        // BASIC FIELDS
        movie.Name = vm.Movie.Name;
        movie.Description = vm.Movie.Description;
        movie.Price = vm.Movie.Price;
        movie.ReleaseDate = vm.Movie.ReleaseDate;
        movie.Status = vm.Movie.Status;

        // MAIN IMAGE
        if (vm.MainImg != null)
        {
            if (!string.IsNullOrEmpty(movie.MainImage))
                _fileService.Delete(movie.MainImage, FileType.Movie);

            movie.MainImage = await _fileService.SaveImageAsync(vm.MainImg, FileType.Movie, ct);
        }

        // ================= IMAGES =================
        if (vm.Gallery?.Any() == true)
        {
            var newImages = new List<MovieImage>();

            foreach (var img in vm.Gallery.Where(f => f.Length > 0))
            {
                var fileName = await _fileService.SaveImageAsync(img, FileType.Movie, ct);

                newImages.Add(new MovieImage
                {
                    MovieId = movie.Id,
                    ImageUrl = fileName
                });
            }

            await _movieImagesRepo.AddRangeAsync(newImages, ct);
        }

        // ================= ACTORS =================
        var oldActors = _movieActorsRepo.Get()
            .Where(x => x.MovieId == movie.Id);

        _movieActorsRepo.RemoveRange(oldActors);

        if (vm.SelectedActors?.Any() == true)
        {
            var newActors = vm.SelectedActors.Select(id => new MovieActor
            {
                MovieId = movie.Id,
                ActorId = id
            });

            await _movieActorsRepo.AddRangeAsync(newActors, ct);
        }

        // ================= CATEGORIES =================
        var oldCats = _movieCategoriesRepo.Get()
            .Where(x => x.MovieId == movie.Id);

        _movieCategoriesRepo.RemoveRange(oldCats);

        if (vm.SelectedCategories?.Any() == true)
        {
            var newCats = vm.SelectedCategories.Select(id => new MovieCategory
            {
                MovieId = movie.Id,
                CategoryId = id
            });

            await _movieCategoriesRepo.AddRangeAsync(newCats, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteImageAsync(int id, CancellationToken ct = default)
    {
        var image = await _movieImagesRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (image == null)
            throw new KeyNotFoundException("Image not found");

        _fileService.Delete(image.ImageUrl, FileType.Movie);

        _movieImagesRepo.Remove(image);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteMovieAsync(int id, CancellationToken ct = default)
    {
        var movie = await _movieRepo.Get()
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (movie == null)
            throw new KeyNotFoundException("Movie not found");

        // ================= DELETE FILES =================
        if (!string.IsNullOrEmpty(movie.MainImage))
            _fileService.Delete(movie.MainImage, FileType.Movie);

        foreach (var img in movie.Images)
        {
            _fileService.Delete(img.ImageUrl, FileType.Movie);
        }

        // ================= DELETE RELATIONS =================
        var actors = _movieActorsRepo.Get()
            .Where(x => x.MovieId == id);

        var cats = _movieCategoriesRepo.Get()
            .Where(x => x.MovieId == id);

        _movieActorsRepo.RemoveRange(actors);
        _movieCategoriesRepo.RemoveRange(cats);

        _movieImagesRepo.RemoveRange(movie.Images);

        // ================= DELETE MOVIE =================
        _movieRepo.Remove(movie);

        await _unitOfWork.SaveChangesAsync(ct);
    }
}