namespace P03_Cinema.Services;

public static class ActorService
{
    private static readonly string _actorPath = "wwwroot/Assets/Admin/img/actor";

    public static string SaveImage(IFormFile file)
    {
        if (file == null || file.Length == 0) return string.Empty;

        if (!Directory.Exists(_actorPath))
            Directory.CreateDirectory(_actorPath);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(_actorPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return fileName;
    }

    public static void DeleteOldImage(string oldFile)
    {
        if (string.IsNullOrEmpty(oldFile)) return;

        var path = Path.Combine(_actorPath, oldFile);
        if (File.Exists(path))
            File.Delete(path);
    }
}