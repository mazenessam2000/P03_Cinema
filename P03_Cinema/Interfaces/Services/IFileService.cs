namespace P03_Cinema.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile file, FileType type, CancellationToken ct = default);
        void Delete(string fileName, FileType type);
    }
}
