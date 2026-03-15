namespace MiniSchoolSystem.Implementation.Interfaces
{
    public interface IFileService
    {
        Task<string?>SaveFileAsync(IFormFile file, string FolderName);
    }
}
