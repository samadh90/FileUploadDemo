using FileUploadDemo.Data;
using FileUploadDemo.Models;

namespace FileUploadDemo.Repositories;

public class FileRepository : IFileRepository
{
    private readonly FileUploadDbContext _context;

    public FileRepository(FileUploadDbContext content)
    {
        _context = content;
    }

    public FileModel Create(FileModel file)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                if (file == null) throw new ArgumentNullException("file");

                if (file.Data == null || file.Data.Length == 0) throw new Exception("The file data cannot be empty.");

                var newFile = new FileModel
                {
                    ID = Guid.NewGuid(),
                    MimeType = file.MimeType,
                    Data = file.Data,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = null
                };

                _context.Files.Add(newFile);
                _context.SaveChanges();

                transaction.Commit();
                return newFile;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
    }

    public IQueryable<FileModel> GetAll()
    {
        return _context.Files.AsQueryable();
    }

    public IQueryable<FileModel> GetById(Guid id)
    {
        return _context.Files.AsQueryable().Where(f => f.ID == id);
    }

    public void Update(FileModel file)
    {
        _context.Files.Update(file);
        _context.SaveChanges();
    }

    public void Delete(FileModel file)
    {
        _context.Files.Remove(file);
        _context.SaveChanges();
    }
}