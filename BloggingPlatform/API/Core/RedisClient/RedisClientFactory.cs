namespace API.Core.RedisClient
{
    public class RedisClientFactory
    {
        public static RedisClient CreateClient()
        {
            return new RedisClient("localhost", 6379, "");
        }
    }
}
