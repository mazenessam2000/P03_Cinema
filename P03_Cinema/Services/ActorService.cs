using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Services;

public class ActorService(
    IRepository<Actor> actorRepo,
    IRepository<MovieActor> movieActorRepo,
    IUnitOfWork unitOfWork,
    IFileService fileService) : IActorService
{
    private readonly IFileService _fileService = fileService;
    private readonly IRepository<Actor> _actorRepo = actorRepo;
    private readonly IRepository<MovieActor> _movieActorRepo = movieActorRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ActorIndexVM> GetActorsPageAsync(int page = 1, int pageSize = 8, string q = "", CancellationToken ct = default)
    {
        var query = _actorRepo.Get().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.FullName.Contains(q));

        int totalActors = await query.CountAsync(ct);
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalActors / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var actors = await query
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var actorIds = actors.Select(m => m.Id).ToList();

        var movieCounts = await _movieActorRepo.Get()
            .Where(x => actorIds.Contains(x.ActorId))
            .GroupBy(x => x.ActorId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);

        return new ActorIndexVM
        {
            Actors = actors,
            ActorMovieCounts = movieCounts,
            CurrentPage = page,
            TotalPages = totalPages,
            SearchQuery = q
        };
    }



    public async Task AddAsync(ActorCreateVM vm, CancellationToken ct = default)
    {
        if (vm.Img != null)
        {
            vm.Actor.ImageUrl = await _fileService.SaveImageAsync(vm.Img, FileType.Actor, ct);
        }

        await _actorRepo.AddAsync(vm.Actor, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ActorUpdateVM vm, CancellationToken ct = default)
    {
        var actor = await _actorRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == vm.Actor.Id, ct)
            ?? throw new KeyNotFoundException($"Actor with id {vm.Actor.Id} not found");

        actor.FullName = vm.Actor.FullName;
        actor.Bio = vm.Actor.Bio;
        actor.BirthDate = vm.Actor.BirthDate;

        if (vm.Img != null)
        {
            var oldImage = actor.ImageUrl;

            var newImage = await _fileService.SaveImageAsync(vm.Img, FileType.Actor, ct);
            actor.ImageUrl = newImage;

            if (!string.IsNullOrEmpty(oldImage))
            {
                _fileService.Delete(oldImage, FileType.Actor);
            }
        }

        _actorRepo.Update(actor);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var actor = await FindByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Actor with id {id} not found");

        var relations = await _movieActorRepo.Get()
            .Where(x => x.ActorId == id)
            .ToListAsync(ct);

        _movieActorRepo.RemoveRange(relations);

        if (!string.IsNullOrEmpty(actor.ImageUrl))
        {
            _fileService.Delete(actor.ImageUrl, FileType.Actor);
        }

        _actorRepo.Remove(actor);
        await _unitOfWork.SaveChangesAsync(ct);
    }
    public async Task<Actor?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        return await _actorRepo.GetByIdAsync(id, ct);
    }
}