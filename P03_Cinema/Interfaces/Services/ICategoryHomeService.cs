namespace P03_Cinema.Interfaces.Services;

public interface ICategoryHomeService
{
    Task<List<CategoryVM>> GetActiveCategoriesAsync();
}