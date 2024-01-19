using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace com.tweetapp.Models
{
    public class Reply
    {
        public ObjectId Id { get; set; }
        [Required]
        [MaxLength(144)]
        public string reply { get; set; }
        public string replyDate { get; set; }
        [Required]
        public string userId { get; set; }
        public string tweetId { get; set; }
        public string[] tags { get; set; }
    }
}
