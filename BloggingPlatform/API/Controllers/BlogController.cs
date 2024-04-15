using Amazon.Runtime.Internal;
using API.Core.Models;
using API.Core.RedisClient;
using API.Core.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using API.Scripts;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class BlogController : ControllerBase
{
    private readonly BlogService _blogService;
    private readonly RedisClient _redisClient;


    public BlogController(BlogService blogService, RedisClient redisClient)
    {
        _blogService = blogService;
        _redisClient = redisClient;
        _redisClient.Connect();
    }

    [HttpPost]
    public IActionResult Post([FromBody] Blog blog)
    {
        var postResult = _blogService.saveBlog(blog);

        if (postResult)
        {
            _redisClient.StoreString(blog._id, blog); // blog is json serialized here
            return Ok("Post was successfully added to the database");
        }
        return BadRequest("Post was not added to the database");
    }

    [HttpGet]
    public IActionResult GetBlogById(string id)
    {
        // Check Redis cache first for result.
        var cachedBlog = _redisClient.GetString(id);

        if (cachedBlog != null)
        {
            Console.WriteLine("Returned result from Redis cache");
            var blog = JsonSerializer.Deserialize<Blog>(cachedBlog);
            return Ok(blog);
        }
        else
        {
            // If its not in the cache, get it from the database
            var result = _blogService.getBlog(id);

            if (result != null)
            {
                Console.WriteLine("Returned result from Mongo");
                _redisClient.StoreString(id, result);
                return Ok(result);
            }
            else
            {
                return BadRequest("result is null");
            }
        }
    }

    //[HttpGet]
    //public IActionResult GetBlogByIdRateLimited(string id)
    //{
    //    var script = RateLimiterScript;
    //    var result =
    //        await _database.ScriptEvaluateAsync(script, new { key = new RedisKey(key), expiry = 60, maxRequests = 2 });
    //    if ((int)result == 1)
    //        return new StatusCodeResult(429);

    //    return Ok("Value of key is: " + _database.StringGet(key));

    //}
}