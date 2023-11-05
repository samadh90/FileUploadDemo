using FileUploadDemo.Data;
using FileUploadDemo.Models;
using FileUploadDemo.Repositories;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace FileUploadDemo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure OData Model
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<FileModel>("Files");

        // Add DbContext
        builder.Services.AddDbContext<FileUploadDbContext>();

        // Add OData services with configuration
        builder.Services.AddControllers()
            .AddOData(options => options
                .Select()
                .Filter()
                .OrderBy()
                .Expand()
                .Count()
                .SetMaxTop(null)
                .EnableQueryFeatures()
                .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

        // Configure Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add Scoped Services
        builder.Services.AddScoped<IFileRepository, FileRepository>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
