using com.tweetapp.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace com.tweetapp.MongoRepository
{
    public class FilterDefinitions
    {
        public FilterDefinition<User> findUserByEmailOrUsername(Login login)
        {
            return Builders<User>.Filter.Or(
                Builders<User>.Filter.Eq(u => u.email, login.loginId),
                Builders<User>.Filter.Eq(u => u.username, login.loginId)
            );
        }
        public FilterDefinition<User> findUserById(string id)
        {
            return Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse(id));
        }

        public FilterDefinition<User> findExistingUsers(User user)
        {
            return Builders<User>.Filter.Or(
                Builders<User>.Filter.Eq(u => u.email, user.email),
                Builders<User>.Filter.Eq(u => u.username, user.username)
            );
        }

        public FilterDefinition<User> findUsersByUsername(string username)
        {
            return Builders<User>.Filter.Eq(u => u.username, username);
        }

        public FilterDefinition<User> findUsersByPartialUsername(string username)
        {
            return Builders<User>.Filter.Regex(u => u.username, username);
        }

        public FilterDefinition<Tweet> findTweetsByUserId(string userId)
        {
            return Builders<Tweet>.Filter.Eq(t => t.userId, userId);
        }

        public FilterDefinition<Tweet> findTweetById(string id)
        {
            return Builders<Tweet>.Filter.Eq(t => t.Id, ObjectId.Parse(id));
        }

        public FilterDefinition<Reply> findReplyByTweetId(string tweetId)
        {
            return Builders<Reply>.Filter.Eq(r => r.tweetId, tweetId);
        }
    }
}
