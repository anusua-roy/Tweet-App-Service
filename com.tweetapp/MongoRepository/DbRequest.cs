using com.tweetapp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.MongoRepository
{
    public class DbRequest : IDbRequest
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DbRequest> _logger;
        private readonly FilterDefinitions _filterDefinitions;
        private readonly IMongoCollection<Tweet> _tweetsCollection;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Reply> _repliesCollection;
        private readonly MongoClient _client;
        public DbRequest(IConfiguration configuration, ILogger<DbRequest> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _filterDefinitions = new FilterDefinitions();
            _client = new MongoClient(_configuration.GetConnectionString("TweetAppConnectionString"));
            _tweetsCollection = _client.GetDatabase("TweetApp").GetCollection<Tweet>("Tweets");
            _usersCollection = _client.GetDatabase("TweetApp").GetCollection<User>("Users");
            _repliesCollection = _client.GetDatabase("TweetApp").GetCollection<Reply>("Replies");
        }

        private User toUserModel(string data)
        {
            BsonDocument bsonDoc = BsonDocument.Parse(data);
            User user = new User();
            user.Id = ObjectId.Parse(bsonDoc["Id"].AsString);
            user.firstName = bsonDoc["firstName"].AsString;
            user.lastName = bsonDoc["lastName"].AsString;
            user.email = bsonDoc["email"].AsString;
            user.username = bsonDoc["username"].AsString;
            user.password = bsonDoc["password"].AsString;
            user.contact = bsonDoc["contact"].AsString;
            return user;
        }
        private Tweet toTweetModel(string data)
        {
            BsonDocument bsonDoc = BsonDocument.Parse(data);
            Tweet tweet = new Tweet();
            tweet.Id = ObjectId.Parse(bsonDoc["Id"].AsString);
            tweet.tweet = bsonDoc["tweet"].AsString;
            tweet.tweetDate = bsonDoc["tweetDate"].AsString;
            tweet.userId = bsonDoc["userId"].AsString;
            tweet.likeCount = bsonDoc["likeCount"].AsInt32;
            tweet.tags = bsonDoc["tags"].AsBsonArray.Select(tag => tag.AsString).ToArray();
            return tweet;
        }
        private Reply toReplyModel(string data)
        {
            BsonDocument bsonDoc = BsonDocument.Parse(data);
            Reply reply = new Reply();
            reply.Id = ObjectId.Parse(bsonDoc["Id"].AsString);
            reply.reply = bsonDoc["reply"].AsString;
            reply.replyDate = bsonDoc["replyDate"].AsString;
            reply.userId = bsonDoc["userId"].AsString;
            reply.tweetId = bsonDoc["tweetId"].AsString;
            reply.tags = bsonDoc["tags"].AsBsonArray.Select(tag => tag.AsString).ToArray();
            return reply;
        }
        public async Task<bool> processRequest(string requestType, string data)
        {
            try
            {
                if (requestType == Global.REQUEST_TYPES[0])
                {
                    User user = toUserModel(data);
                    await _usersCollection.InsertOneAsync(user);
                    return true;
                }

                else if (requestType == Global.REQUEST_TYPES[1])
                {
                    Tweet tweet = toTweetModel(data);
                    await _tweetsCollection.InsertOneAsync(tweet);
                    return true;
                }

                else if (requestType == Global.REQUEST_TYPES[2])
                {
                    Tweet tweet = toTweetModel(data);
                    UpdateDefinition<Tweet> update = Builders<Tweet>.Update.Set(t => t.tweet, tweet.tweet).Set(t => t.tags, tweet.tags);
                    await _tweetsCollection.UpdateOneAsync(_filterDefinitions.findTweetById(tweet.Id.ToString()), update);
                    return true;
                }

                else if (requestType == Global.REQUEST_TYPES[3])
                {
                    Tweet tweet = toTweetModel(data);
                    await _tweetsCollection.DeleteOneAsync(_filterDefinitions.findTweetById(tweet.Id.ToString()));
                    await _repliesCollection.DeleteManyAsync(_filterDefinitions.findReplyByTweetId(tweet.Id.ToString()));

                    return true;
                }

                else if (requestType == Global.REQUEST_TYPES[4])
                {
                    Tweet tweet = toTweetModel(data);
                    UpdateDefinition<Tweet> update = Builders<Tweet>.Update.Set(t => t.likeCount, tweet.likeCount);
                    await _tweetsCollection.UpdateOneAsync(_filterDefinitions.findTweetById(tweet.Id.ToString()), update);
                    return true;
                }

                else if (requestType == Global.REQUEST_TYPES[5])
                {
                    Reply reply = toReplyModel(data);
                    await _repliesCollection.InsertOneAsync(reply);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }
    }
}
