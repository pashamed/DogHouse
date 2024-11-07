namespace DogHouse.Web
{
    public class RateLimitOptions
    {
        public int PermitLimit { get; set; }
        public TimeSpan Window { get; set; }
        public int QueueLimit { get; set; }
    }
}