using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace com.tweetapp.Models
{
    public class UsersView
    {
        public string userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string contact { get; set; }
        public UsersView() { }

        public UsersView(User user)
        {
            this.userId = user.Id.ToString();
            this.firstName = user.firstName;
            this.lastName = user.lastName;
            this.email = user.email;
            this.username = user.username;
            this.contact = user.contact;
        }
    }
}
