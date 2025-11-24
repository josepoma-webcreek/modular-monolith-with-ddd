namespace CompanyName.MyMeetings.BuildingBlocks.Infrastructure.Caching
{
    public class CacheConfiguration
    {
        public string CacheType { get; set; } = "InMemory";

        public string RedisConnectionString { get; set; } = string.Empty;
    }
}
