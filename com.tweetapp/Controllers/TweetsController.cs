using com.tweetapp;
using com.tweetapp.Kafka;
using com.tweetapp.Models;
using com.tweetapp.MongoRepository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace com.tweetapp.Controllers
{
    [Route(V)]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private const string V = "api/v1.0/tweets/";
        private readonly IRepository _repository;
        private readonly IProducer _procuder;
        public TweetsController(IRepository repository, IProducer producer)
        {
            _repository = repository;
            _procuder = producer;
        }

        private bool _checkTagLengths(string[] tags)
        {
            foreach (string tag in tags)
            {
                if (tag.Length > 50)
                {
                    return false;
                }
            }
            return true;
        }

        [HttpGet("all")]
        public IActionResult GetAllTweets()
        {
            try
            {
                return Ok(_repository.GetTweets());
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");
            }
        }

        [HttpGet("{username}")]
        public IActionResult GetTweetsByUsername(string username)
        {
            try
            {
                User user = _repository.GetUsersByUsername(username);
                if (user != null)
                {
                    return Ok(_repository.GetTweetsByUser(user));
                }
                else
                {
                    return NotFound("No users found!");
                }
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");
            }
        }

        [HttpPost("{username}/add")]
        public async Task<IActionResult> CreateTweet(string username, [FromBody] Tweet tweet)
        {
            if (!ModelState.IsValid || !_checkTagLengths(tweet.tags))
            {
                return BadRequest();
            }

            try
            {
                User user = _repository.GetUsersByUsername(username);
                if (user != null)
                {
                    tweet.tweetDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                    tweet.userId = user.Id.ToString();
                    tweet.likeCount = 0;
                    string data = JsonConvert.SerializeObject(tweet);
                    if (await _procuder.SendRequestToKafkaAsync(Global.REQUEST_TYPES[1], data))
                        return StatusCode(201, "Tweet posted successfully!");
                    else
                        return StatusCode(500, "Something went wrong, could not post the tweet. Please try again.");
                }
                else
                {
                    return NotFound("No user found!");
                }
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");
            }
        }

        [HttpPut("{username}/update/{id}")]
        public async Task<IActionResult> UpdateTweet(string username, string id, [FromBody] Tweet tweet)
        {
            if (!ModelState.IsValid || !_checkTagLengths(tweet.tags))
            {
                return BadRequest();
            }

            try
            {
                User user = _repository.GetUsersByUsername(username);

                if (user != null)
                {
                    Tweet tweetObject = _repository.GetTweetById(id);

                    if (tweetObject != null)
                    {
                        tweetObject.tweet = tweet.tweet;
                        tweetObject.tags = tweet.tags;
                        string data = JsonConvert.SerializeObject(tweetObject);
                        if (await _procuder.SendRequestToKafkaAsync(Global.REQUEST_TYPES[2], data))
                            return Ok("Tweet updated successfully!");
                        else return StatusCode(500, "Something went wrong, couldn't update tweet. Please try again.");
                    }
                    else
                    {
                        return NotFound("Tweet not found!");
                    }
                }
                else
                {
                    return NotFound("No user found!");
                }
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");

            }
        }

        [HttpDelete("{username}/delete/{id}")]
        public async Task<IActionResult> DeleteTweet(string username, string id)
        {
            try
            {
                User user = _repository.GetUsersByUsername(username);

                if (user != null)
                {
                    Tweet tweetObject = _repository.GetTweetById(id);

                    if (tweetObject != null)
                    {
                        string data = JsonConvert.SerializeObject(tweetObject);
                        if (await _procuder.SendRequestToKafkaAsync(Global.REQUEST_TYPES[3], data))
                            return StatusCode(204, "Tweet deleted successfully!");
                        else return StatusCode(500, "Something went wrong, couldn't delete tweet. Please try again.");
                    }
                    else
                    {
                        return NotFound("Tweet not found!");
                    }
                }
                else
                {
                    return NotFound("No user found!");
                }
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");

            }
        }

        [HttpPut("{username}/like/{id}")]
        public async Task<IActionResult> LikeTweet(string username, string id, int like)
        {
            if (like != 1 && like != -1)
            {
                return BadRequest("Invalid data passed.");
            }
            try
            {
                User user = _repository.GetUsersByUsername(username);

                if (user != null)
                {
                    Tweet tweetObject = _repository.GetTweetById(id);

                    if (tweetObject != null)
                    {
                        tweetObject.likeCount += like;
                        string data = JsonConvert.SerializeObject(tweetObject);
                        if (await _procuder.SendRequestToKafkaAsync(Global.REQUEST_TYPES[4], data))
                            return Ok();
                        else return StatusCode(500, "Something went wrong, couldn't update tweet. Please try again.");
                    }
                    else
                    {
                        return NotFound("Tweet not found!");
                    }
                }
                else
                {
                    return NotFound("No user found!");
                }
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");

            }
        }

    }
}
