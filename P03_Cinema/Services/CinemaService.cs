namespace P03_Cinema.Services;

public static class CinemaService
{
    public static string CreateFile(IFormFile Img)
    {
        var fileName = 
            $"Cinema-{Guid.NewGuid()}{Path.GetExtension(Img.FileName)}";

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Admin\\img\\cinema", fileName);

        using (var stream = File.Create(filePath))
        {
            Img.CopyTo(stream);
        }

        return fileName;
    }

    public static string GetOldFilePath(string oldFileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Admin\\img\\cinema", oldFileName);

        return filePath;
    }
}