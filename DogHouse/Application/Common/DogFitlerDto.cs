namespace DogHouse.Application.Common
{
    public class DogFitlerDto
    {
        public List<string>? Attributes { get; set; }
        public List<string>? Orders { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}