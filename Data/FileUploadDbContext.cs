using FileUploadDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace FileUploadDemo.Data;

public class FileUploadDbContext : DbContext
{
    public DbSet<FileModel> Files { get; set; }
    protected readonly IConfiguration Configuration;

    public FileUploadDbContext(DbContextOptions<FileUploadDbContext> options, IConfiguration configuration)
        : base(options)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // connect to sqlite database
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("SQLiteDb"));
    }
}
