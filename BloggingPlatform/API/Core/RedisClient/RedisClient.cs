using API.Core.Models;
using MongoDB.Bson.IO;
using StackExchange.Redis;
using System.Text.Json;

namespace API.Core.RedisClient
{
    public class RedisClient
    {
        private readonly string _hostname;
        private readonly int _port;
        private readonly string _password;

        private ConnectionMultiplexer redis;

        public RedisClient(string hostname, int port, string password)
        {
            _hostname = hostname;
            _port = port;
            _password = password;
        }

        public void Connect()
        {
            string connectionString = $"{_hostname}:{_port},password={_password}";
            redis = ConnectionMultiplexer.Connect(connectionString);
        }

        public IDatabase GetDatabase()
        {
            return redis.GetDatabase();
        }

        public void StoreString(string key, Blog blog)
        {
            var json = JsonSerializer.Serialize(blog);
            var db = GetDatabase();
            db.StringSet(key, json);
        }

        public string? GetString(string key)
        {
            var db = GetDatabase();
            return db.StringGet(new RedisKey(key));
        }

        public void RemoveString(string key)
        {
            var db = GetDatabase();
            db.KeyDelete(key);
        }
    }
}
