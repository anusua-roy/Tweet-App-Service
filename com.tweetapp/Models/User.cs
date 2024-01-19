using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace com.tweetapp.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        [Phone]
        public string contact { get; set; }
    }
}
