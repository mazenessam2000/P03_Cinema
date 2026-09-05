using Microsoft.Extensions.Options;

namespace P03_Cinema.Services;

public class FileService : IFileService
{
    private readonly FileSettings _settings;

    public async Task<string> SaveImageAsync(IFormFile file, FileType type, CancellationToken ct = default)
    {
        if (file == null) return null!;

        string rootPath = GetFullPath(GetFolder(type));

        string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        string fullPath = Path.Combine(rootPath, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return fileName;
    }

    public void Delete(string fileName, FileType type)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        string rootPath = GetFullPath(GetFolder(type));
        string fullPath = Path.Combine(rootPath, fileName);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    public FileService(IOptions<FileSettings> options)
    {
        _settings = options.Value;
    }

    private string GetFullPath(string relativePath)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        return fullPath;
    }

    private string GetFolder(FileType type)
    {
        return type switch
        {
            FileType.Category => _settings.CategoryPath,
            FileType.Actor => _settings.ActorPath,
            FileType.Cinema => _settings.CinemaPath,
            FileType.Movie => _settings.MoviePath,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}