
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using StreamingVideoIndexer.Infra.DatabaseContext;
using StreamingVideoWebApi.Core.Interfaces.Repositories;
using StreamingVideoWebApi.Core.Interfaces.Services;
using StreamingVideoWebApi.Core.Services;
using StreamingVideoWebApi.Infra.MappingProfiles;
using StreamingVideoWebApi.Infra.Repositories;
using StreamingVideoWebApi.Infra.Services;
using StreamingVideoWebApi.Infra.Settings;

namespace StreamingVideoWebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.Configure<S3Config>(builder.Configuration.GetSection("S3Config"));

        // Add services to the container.
        builder.Services.AddScoped<IIndexedFilesRepository, IndexedFilesRepository>();
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Config = configuration.GetSection("S3Config").Get<S3Config>();

            var config = new AmazonS3Config
            {
                ServiceURL = s3Config?.Url,
                ForcePathStyle = true
            };


            return new AmazonS3Client(s3Config?.AccessKey, s3Config?.SecretKey, config);
        });

        builder.Services.AddSingleton<IS3StorageService, S3StorageService>();
        builder.Services.AddScoped<IVideosService, VideosService>();
        builder.Services.AddAutoMapper(typeof(IndexedVideoProfile));

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("StreamingVideoIndexer"));
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "StreamingVideoWebApi:";
        });

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

        // allow all origins
        app.UseCors(builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });

        app.Run();
    }
}
