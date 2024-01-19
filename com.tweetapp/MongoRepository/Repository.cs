using com.tweetapp.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.MongoRepository
{
    public class Repository : IRepository
    {
        private readonly IConfiguration _configuration;
        private readonly FilterDefinitions _filterDefinitions;
        private readonly IMongoCollection<Tweet> _tweetsCollection;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Reply> _repliesCollection;

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration;
            _filterDefinitions = new FilterDefinitions();
            MongoClient _client = new MongoClient(_configuration.GetConnectionString("TweetAppConnectionString"));
            _tweetsCollection = _client.GetDatabase("TweetApp").GetCollection<Tweet>("Tweets");
            _usersCollection = _client.GetDatabase("TweetApp").GetCollection<User>("Users");
            _repliesCollection = _client.GetDatabase("TweetApp").GetCollection<Reply>("Replies");
        }

        public int CheckExistingUser(User user)
        {
            return _usersCollection.Find(_filterDefinitions.findExistingUsers(user)).ToList().Count;
        }

        public dynamic GetTweetById(string id)
        {
            return _tweetsCollection.Find(_filterDefinitions.findTweetById(id)).FirstOrDefault();
        }

        public List<TweetsView> GetTweets()
        {
            List<TweetsView> tweetsViewList = new();
            List<Tweet> tweetsList = _tweetsCollection.AsQueryable().ToList();
            foreach (Tweet tweet in tweetsList)
            {
                UsersView user = new UsersView(_usersCollection.Find(_filterDefinitions.findUserById(tweet.userId)).FirstOrDefault());
                FilterDefinition<Reply> findRepliesByTweetId = Builders<Reply>.Filter.Eq(r => r.tweetId, tweet.Id.ToString());
                List<RepliesView> repliesViewList = new();
                List<Reply> repliesList =  _repliesCollection.Find(findRepliesByTweetId).ToList();
                foreach(Reply reply in repliesList)
                {
                    UsersView repliedBy = new UsersView(_usersCollection.Find(_filterDefinitions.findUserById(reply.userId)).FirstOrDefault());
                    repliesViewList.Add(new RepliesView(reply, repliedBy));
                }
                tweetsViewList.Add(new TweetsView(tweet, user, repliesViewList));
            }
            return tweetsViewList;
        }

        public List<TweetsView> GetTweetsByUser(User user)
        {
            List<TweetsView> tweetsViewList = new();
            List<Tweet> tweetsList = _tweetsCollection.Find(_filterDefinitions.findTweetsByUserId(user.Id.ToString())).ToList();
            foreach (Tweet tweet in tweetsList)
            {
                UsersView userView = new(user);
                FilterDefinition<Reply> findRepliesByTweetId = Builders<Reply>.Filter.Eq(r => r.tweetId, tweet.Id.ToString());
                List<RepliesView> repliesViewList = new();
                List<Reply> repliesList = _repliesCollection.Find(findRepliesByTweetId).ToList();
                foreach (Reply reply in repliesList)
                {
                    UsersView repliedBy = new(_usersCollection.Find(_filterDefinitions.findUserById(reply.userId)).FirstOrDefault());
                    repliesViewList.Add(new RepliesView(reply, repliedBy));
                }
                tweetsViewList.Add(new TweetsView(tweet, userView, repliesViewList));
            }
            return tweetsViewList;
        }

        public dynamic GetUserByEmailOrUsername(Login login)
        {
            return _usersCollection.Find(_filterDefinitions.findUserByEmailOrUsername(login)).FirstOrDefault();
        }

        public List<UsersView> GetUsers()
        {
            List<UsersView> usersViewList = new();
            List<User> usersList = _usersCollection.AsQueryable().ToList();
            foreach (User user in usersList)
            {
                usersViewList.Add(new UsersView(user));
            }
            return usersViewList;
        }

        public dynamic GetUsersByUsername(string username)
        {
            return _usersCollection.Find(_filterDefinitions.findUsersByUsername(username)).FirstOrDefault();
        }

        public async Task<List<UsersView>> SearchUsersByUsername(string username)
        {
            List<UsersView> usersViewList = new();
            List<User> usersList = await _usersCollection.Find(_filterDefinitions.findUsersByPartialUsername(username)).ToListAsync();
            foreach (User user in usersList)
            {
                usersViewList.Add(new UsersView(user));
            }
            return usersViewList;
        }

        public async Task<bool> UpdateUserPassword(string username, string password)
        {
            try
            {
                UpdateDefinition<User> update = Builders<User>.Update.Set(u => u.password, password);
                await _usersCollection.FindOneAndUpdateAsync(_filterDefinitions.findUsersByUsername(username), update);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
