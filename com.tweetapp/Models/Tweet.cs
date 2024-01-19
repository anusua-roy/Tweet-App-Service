using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace com.tweetapp.Models
{
    public class Tweet
    {
        public ObjectId Id { get; set; }
        [Required]
        [MaxLength(144)]
        public string tweet { get; set; }
        public string tweetDate { get; set; }
        public string userId { get; set; }
        public int likeCount { get; set; }
        public string[] tags { get; set; }
    }
}
