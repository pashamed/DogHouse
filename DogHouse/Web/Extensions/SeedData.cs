using DogHouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DogHouse.Web.Extensions
{
    public static class SeedData
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dog>().HasData(
               new Dog { Id = 1, Name = "Neo", Colors = new List<string> { "Red", "Amber" }, TailLength = 22.0, Weight = 32.0 },
               new Dog { Id = 2, Name = "Jessy", Colors = new List<string> { "Black", "White" }, TailLength = 7, Weight = 30.0 }
           );
        }
    }
}