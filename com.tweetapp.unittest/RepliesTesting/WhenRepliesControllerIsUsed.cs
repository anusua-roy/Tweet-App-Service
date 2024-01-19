using com.tweetapp.Controllers;
using com.tweetapp.Kafka;
using com.tweetapp.Models;
using com.tweetapp.MongoRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace com.tweetapp.unittest.RepliesTesting
{
    internal class WhenRepliesControllerIsUsed
    {
        static Mock<IRepository> mockRepository = new Mock<IRepository>();
        static Mock<IProducer> mockProducer = new Mock<IProducer>();
        RepliesController repliesController = new RepliesController(mockRepository.Object, mockProducer.Object);
        [Test]
        public async Task UpdateTweetReplyTest_InvalidInput()
        {
            mockRepository.Setup(p => p.GetTweetById("62e390ae0b48b96ce8b11fe9")).Returns(null);
            Reply reply = new();
            var result = await repliesController.UpdateTweetReply("anusua", "62e390ae0b48b96ce8b11fe9", reply) as NotFoundObjectResult;
            Assert.That(result.Value, Is.EqualTo("Tweet not found!"));
        }
    }
}
