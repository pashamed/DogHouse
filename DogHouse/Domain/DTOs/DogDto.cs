namespace DogHouse.Domain.DTOs
{
    public class DogDto
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double TailLength { get; set; }
        public double Weight { get; set; }
    }
}