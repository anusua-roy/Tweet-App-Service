using com.tweetapp.Kafka;
using com.tweetapp.Models;
using com.tweetapp.MongoRepository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.tweetapp.Controllers
{
    [Route(V)]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private const string V = "api/v1.0/tweets/";
        private readonly IRepository _repository;
        private readonly IProducer _procuder;
        public UsersController(IRepository repository, IProducer producer)
        {
            _repository = repository;
            _procuder = producer;
        }
        [HttpGet("users/all")]
        public IActionResult GetAllUsers()
        {
            try
            {
                return Ok(_repository.GetUsers());
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");
            }
        }

        [HttpGet("users/login")]
        public IActionResult LoginUser(string loginId, string auth)
        {
            try
            {
                Login loginDetails = new(loginId, auth);
                dynamic user = _repository.GetUserByEmailOrUsername(loginDetails);
                if (user == null)
                    return NotFound("User does not exist!");
                if (user.password == auth)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest("Invalid Credentials");
                }
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");
            }
        }

        [HttpPost("users/register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                if (_repository.CheckExistingUser(user) > 0)
                {
                    return BadRequest("Email and Username should be unique");
                }
                string data = JsonConvert.SerializeObject(user);
                if (await _procuder.SendRequestToKafkaAsync(Global.REQUEST_TYPES[0], data))
                    return StatusCode(201, "User Registered Successfully!");
                else
                    return StatusCode(500, "Something went wrong, could not register user. Please try again.");
            }
            catch
            {
                return StatusCode(500, "Something went wrong, please try again.");
            }
        }

        [HttpGet("{username}/forgot")]
        public async Task<IActionResult> ForgetPassword(string username, string password)
        {
            try
            {
                User user = _repository.GetUsersByUsername(username);
                if (user != null)
                {
                    if (await _repository.UpdateUserPassword(username, password))
                        return Ok("Password updated successfully!");
                    else return StatusCode(500, "Something went wrong, could not update password. Please try again.");
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

        [HttpGet("user/search/{username}")]
        public async Task<IActionResult> GetUsersByUsername(string username)
        {
            try
            {
                List<UsersView> dbList = await _repository.SearchUsersByUsername(username);
                if (dbList.Count > 0)
                {
                    return Ok(dbList);
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
    }
}
