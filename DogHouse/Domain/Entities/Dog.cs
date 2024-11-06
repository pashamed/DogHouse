namespace DogHouse.Domain.Entities
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Colors { get; set; } = new();
        public double TailLength { get; set; }
        public double Weight { get; set; }
    }
}