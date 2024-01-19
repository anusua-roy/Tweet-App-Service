using com.tweetapp.Controllers;
using com.tweetapp.Kafka;
using com.tweetapp.Models;
using com.tweetapp.MongoRepository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace com.tweetapp.unittest.UsersTesting
{
    internal class WhenUsersControllerIsUsed
    {
        User user = new();
        static Mock<IRepository> mockRepository = new Mock<IRepository>();
        static Mock<IProducer> mockProducer = new Mock<IProducer>();
        UsersController usersController = new UsersController(mockRepository.Object, mockProducer.Object);
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
        public void GetAllUsersTest_ValidInput()
        {
            var result = usersController.GetAllUsers() as OkObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
        [Test]
        public void LoginTest_InvalidInput()
        {
            Login login = new("anusua", "pass123");
            mockRepository.Setup(p => p.GetUserByEmailOrUsername(login)).Returns(null);
            var result = usersController.LoginUser("anusua", "pass123") as NotFoundObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }
    }
}
