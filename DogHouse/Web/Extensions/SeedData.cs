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
              new Dog { Id = 2, Name = "Jessy", Colors = new List<string> { "Black", "White" }, TailLength = 7, Weight = 30.0 },
              new Dog { Id = 3, Name = "Max", Colors = new List<string> { "Brown" }, TailLength = 15.0, Weight = 25.0 },
              new Dog { Id = 4, Name = "Bella", Colors = new List<string> { "Golden" }, TailLength = 20.0, Weight = 28.0 },
              new Dog { Id = 5, Name = "Charlie", Colors = new List<string> { "Black" }, TailLength = 18.0, Weight = 35.0 },
              new Dog { Id = 6, Name = "Lucy", Colors = new List<string> { "White" }, TailLength = 10.0, Weight = 22.0 },
              new Dog { Id = 7, Name = "Cooper", Colors = new List<string> { "Gray" }, TailLength = 12.0, Weight = 27.0 },
              new Dog { Id = 8, Name = "Daisy", Colors = new List<string> { "Brown", "White" }, TailLength = 14.0, Weight = 24.0 },
              new Dog { Id = 9, Name = "Rocky", Colors = new List<string> { "Black", "Brown" }, TailLength = 16.0, Weight = 29.0 },
              new Dog { Id = 10, Name = "Molly", Colors = new List<string> { "Golden", "White" }, TailLength = 19.0, Weight = 26.0 },
              new Dog { Id = 11, Name = "Buddy", Colors = new List<string> { "Red" }, TailLength = 21.0, Weight = 31.0 },
              new Dog { Id = 12, Name = "Lola", Colors = new List<string> { "Amber" }, TailLength = 13.0, Weight = 23.0 }
          );
        }
    }
}