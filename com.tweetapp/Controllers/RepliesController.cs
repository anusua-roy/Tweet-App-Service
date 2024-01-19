using com.tweetapp.Kafka;
using com.tweetapp.Models;
using com.tweetapp.MongoRepository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace com.tweetapp.Controllers
{
    [Route(V)]
    [ApiController]
    public class RepliesController : ControllerBase
    {
        private const string V = "api/v1.0/tweets/";
        private readonly IRepository _repository;
        private readonly IProducer _procuder;
        public RepliesController(IRepository repository, IProducer producer)
        {
            _repository = repository;
            _procuder = producer;
        }

        [HttpPost("{username}/reply/{id}")]
        public async Task<IActionResult> UpdateTweetReply(string username, string id, [FromBody] Reply reply)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                Tweet tweet = _repository.GetTweetById(id);
                if (tweet != null)
                {
                    reply.Id = ObjectId.Empty;
                    reply.tweetId = tweet.Id.ToString();
                    reply.replyDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                    string data = JsonConvert.SerializeObject(reply);
                    if (await _procuder.SendRequestToKafkaAsync(Global.REQUEST_TYPES[5], data))
                        return Ok();
                    else return StatusCode(500, "Something went wrong, could not post your reply. Please try again");
                }
                else
                {
                    return NotFound("Tweet not found!");
                }
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");
            }
        }
    }
}
