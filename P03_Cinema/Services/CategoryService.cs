namespace P03_Cinema.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


public class CategoryService(
    IRepository<Category> categoryRepo,
    IRepository<MovieCategory> movieCategoryRepo,
    IFileService fileService,
    IUnitOfWork unitOfWork) : ICategoryService
{
    private readonly IRepository<Category> _categoryRepo = categoryRepo;
    private readonly IRepository<MovieCategory> _movieCategoryRepo = movieCategoryRepo;
    private readonly IFileService _fileService = fileService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CategoryIndexVM> GetCategoriesPageAsync(int page = 1, int pageSize = 8, string q = "", CancellationToken ct = default)
    {
        var query = _categoryRepo.Get().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Name.Contains(q));

        int totalCategories = await query.CountAsync(ct);
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalCategories / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var categories = await query
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new CategoryIndexVM
        {
            Categories = categories,
            CurrentPage = page,
            TotalPages = totalPages,
            SearchQuery = q
        };
    }


    public async Task AddAsync(CategoryCreateVM vm, CancellationToken ct = default)
    {

        if (vm.Img != null)
            vm.Category.ImageUrl = await _fileService.SaveImageAsync(vm.Img, FileType.Category, ct);

        await _categoryRepo.AddAsync(vm.Category, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CategoryUpdateVM vm, CancellationToken ct = default)
    {


        var category = await _categoryRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == vm.Category.Id, ct)
            ?? throw new KeyNotFoundException($"Category with id {vm.Category.Id} not found");

        category.Name = vm.Category.Name;
        category.Description = vm.Category.Description;
        category.IsActive = vm.Category.IsActive;

        if (vm.Img != null)
        {
            var oldImage = category.ImageUrl;

            var newImage = await _fileService.SaveImageAsync(vm.Img, FileType.Category, ct);
            category.ImageUrl = newImage;

            if (!string.IsNullOrEmpty(oldImage))
            {
                _fileService.Delete(oldImage, FileType.Category);
            }
        }

        _categoryRepo.Update(category);
        await _unitOfWork.SaveChangesAsync(ct);
    }


    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var category = await FindByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Category with id {id} not found");

        var relations = await _movieCategoryRepo.Get()
            .Where(x => x.CategoryId == id)
            .ToListAsync(ct);

        _movieCategoryRepo.RemoveRange(relations);

        if (!string.IsNullOrEmpty(category.ImageUrl))
        {
            _fileService.Delete(category.ImageUrl, FileType.Category);
        }

        _categoryRepo.Remove(category);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<Category?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        return await _categoryRepo.GetByIdAsync(id, ct);
    }
}
