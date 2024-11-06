using DogHouse.Application.Common.Interfaces;
using DogHouse.Application.Common.Mappings;
using DogHouse.Application.Repositories;
using DogHouse.Application.Services;
using DogHouse.Application.Validators;
using DogHouse.Domain.DTOs;
using DogHouse.Infrastructure;
using DogHouse.Infrastructure.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}