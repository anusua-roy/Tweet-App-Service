using com.tweetapp.Controllers;
using com.tweetapp.Kafka;
using com.tweetapp.Models;
using com.tweetapp.MongoRepository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace com.tweetapp.unittest.TweetsTesting
{
    internal class WhenTweetsControllerIsUsed
    {
        User user = new();
        static Mock<IRepository> mockRepository = new Mock<IRepository>();
        static Mock<IProducer> mockProducer = new Mock<IProducer>();
        TweetsController tweetsController = new TweetsController(mockRepository.Object, mockProducer.Object);
        [SetUp]
        public void Setup()
        {
            user = new User
            {
                Id = ObjectId.Parse("62e38755866d95ef4a935b77"),
                firstName = "Anusua",
                lastName = "Roy",
                email = "user1@example.com",
                username = "anusua",
                contact = "0123456789",
                password = "pass12345"
            };
        }
        [Test]
        public void GetAllTweetsTest_ValidInput()
        {
            var result = tweetsController.GetAllTweets() as OkObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
        [Test]
        public async Task LikeTweetTest_InvalidInput()
        {
            var result = await tweetsController.LikeTweet("anusua", "62e390ae0b48b96ce8b11fe9", 4) as BadRequestObjectResult;
            Assert.That(result.Value, Is.EqualTo("Invalid data passed."));
        }
        [Test]
        public void GetTweetsByUsernameTest_ValidInput()
        {
            mockRepository.Setup(p => p.GetUsersByUsername("anusua")).Returns(user);
            var result = tweetsController.GetTweetsByUsername("anusua") as OkObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
        [Test]
        public void GetTweetsByUsernameTest_InvalidInput()
        {
            mockRepository.Setup(p => p.GetUsersByUsername("anusua1234")).Returns(null);
            var result = tweetsController.GetTweetsByUsername("anusua") as NotFoundObjectResult;
            Assert.That(result.Value, Is.EqualTo("No users found!"));
        }
    }
}
