using DogHouse.Application.Common.Interfaces;
using DogHouse.Application.Common.Mappings;
using DogHouse.Application.Repositories;
using DogHouse.Application.Services;
using DogHouse.Application.Validators;
using DogHouse.Domain.DTOs;
using DogHouse.Infrastructure;
using DogHouse.Infrastructure.Interfaces;
using DogHouse.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace DogHouse
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Doghouse")));
            builder.Services.AddAutoMapper(typeof(MapProfile));

            builder.Services.AddScoped<IDogRepository, DogRepository>();
            builder.Services.AddScoped<IDogService,DogService>();
            builder.Services.AddScoped<IValidator<DogDto>, DogValidator>();

            builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimitingSettings"));
            var rateLimitingSettings = builder.Configuration.GetSection("RateLimitingSettings").Get<RateLimitOptions>();
            builder.Services.AddRateLimiter(options =>
            {                
                if (rateLimitingSettings == null)
                {
                    builder.Logging.AddConsole().AddFilter(level => level >= LogLevel.Warning);
                    return;
                }
                options.AddFixedWindowLimiter("Fixed", limiterOptions =>
                {
                    limiterOptions.Window = rateLimitingSettings.Window;
                    limiterOptions.PermitLimit = rateLimitingSettings.PermitLimit;
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.QueueLimit = rateLimitingSettings.QueueLimit;
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Apply pending migrations
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            if (rateLimitingSettings != null)
            {
                app.UseRateLimiter();
            }

            app.Run();
        }
    }
}