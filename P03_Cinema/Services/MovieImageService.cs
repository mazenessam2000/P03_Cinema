namespace P03_Cinema.Services
{
    public static class MovieImageService
    {
        private static readonly string RootPath =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", "Admin", "img", "movie");

        public static string Upload(IFormFile file)
        {
            if (file == null) return null!;

            if (!Directory.Exists(RootPath))
                Directory.CreateDirectory(RootPath);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(RootPath, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }

        public static void Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            string fullPath = Path.Combine(RootPath, fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}