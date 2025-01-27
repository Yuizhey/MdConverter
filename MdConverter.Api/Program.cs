using MdConverter.Application;
using MdConverter.Application.Services;
using MdConverter.Core.Abstractions.Repositories;
using MdConverter.Core.Abstractions.Services;
using MdConverter.DataAccess;
using MdConverter.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MdConverter.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<MdConverterDbContext>(
            options => options.UseNpgsql(builder.Configuration.GetConnectionString("MdConverterDbContext")));
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IDocumentService, DocumentService>();
        builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
        builder.Services.AddScoped<JwtService>();
        builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
        builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("MinioSettings"));
        builder.Services.AddSingleton<MinioService>(); 
        builder.Services.AddControllers();
        builder.Services.AddAuth(builder.Configuration);
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();

        app.Run();
    }
}