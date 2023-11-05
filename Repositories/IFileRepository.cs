using FileUploadDemo.Models;

namespace FileUploadDemo.Repositories
{
    public interface IFileRepository
    {
        FileModel Create(FileModel file);
        void Delete(FileModel file);
        IQueryable<FileModel> GetAll();
        IQueryable<FileModel> GetById(Guid id);
        void Update(FileModel file);
    }
}