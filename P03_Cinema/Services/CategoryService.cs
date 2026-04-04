namespace P03_Cinema.Services;

using Microsoft.AspNetCore.Http;
using System.IO;


public static class CategoryService
{
    /// <summary>
    /// Saves the uploaded category image to wwwroot and returns the file name
    /// </summary>
    public static string SaveImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return string.Empty;

        // Generate unique file name
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        // Set folder path
        var folderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "Assets",
            "Admin",
            "img",
            "category"
        );

        // Create folder if it doesn't exist
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var fullPath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return fileName;
    }

    /// <summary>
    /// Deletes an existing category image
    /// </summary>
    public static void DeleteImage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "Assets",
            "Admin",
            "img",
            "category",
            fileName
        );

        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
