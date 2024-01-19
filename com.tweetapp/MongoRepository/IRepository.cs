using com.tweetapp.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.tweetapp.MongoRepository
{
    public interface IRepository
    {
        List<TweetsView> GetTweets();
        List<UsersView> GetUsers();
        dynamic GetUserByEmailOrUsername(Login login);
        int CheckExistingUser(User user);
        dynamic GetUsersByUsername(string username);
        Task<bool> UpdateUserPassword(string username, string password);
        Task<List<UsersView>> SearchUsersByUsername(string username);
        List<TweetsView> GetTweetsByUser(User user);
        dynamic GetTweetById(string id);
    }
}
