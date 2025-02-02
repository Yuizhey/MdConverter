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

        // Заменяем загрузку конфигурации по умолчанию (appsettings.json) 
        builder.Configuration.AddEnvironmentVariables();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Configure Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // Настройка DbContext с учетом переменных окружения
        var connectionString = builder.Configuration["ConnectionStrings:MdConverterDbContext"];
        builder.Services.AddDbContext<MdConverterDbContext>(
            options => options.UseNpgsql(connectionString));
        
        // Регистрация сервисов и репозиториев
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IDocumentService, DocumentService>();
        builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
        builder.Services.AddScoped<JwtService>();

        // Настройка параметров аутентификации и Minio через переменные окружения
        builder.Services.Configure<AuthSettings>(options =>
        {
            options.SecretKey = builder.Configuration["AUTH:SECRETKEY"];
            options.Expires = TimeSpan.Parse(builder.Configuration["AUTH:EXPIRES"]!);
        });

        builder.Services.Configure<MinioSettings>(options =>
        {
            options.Endpoint = builder.Configuration["MINIO:URL"];
            options.AccessKey = builder.Configuration["MINIO:ACCESSKEY"];
            options.SecretKey = builder.Configuration["MINIO:SECRETKEY"];
            options.BucketName = builder.Configuration["MINIO:BUCKETNAME"];
        });
        
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
        Console.WriteLine(builder.Configuration["AUTH:SECRETKEY"]);
        Console.WriteLine(TimeSpan.Parse(builder.Configuration["AUTH:EXPIRES"]));
    }
}