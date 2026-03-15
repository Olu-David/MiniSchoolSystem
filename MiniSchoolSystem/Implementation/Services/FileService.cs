using MiniSchoolSystem.Implementation.Interfaces;

namespace MiniSchoolSystem.Implementation.Services
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }




        public async Task<string?> SaveFileAsync(IFormFile file, string FolderName)
        {
            if (file == null || file.Length == 0 || FolderName == null)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", FolderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/{FolderName}/{fileName}";
        }
    }
}