using System.Collections.Generic;

namespace com.tweetapp.Models
{
    public class TweetsView
    {
        public string tweetId { get; set; }
        public string tweet { get; set; }
        public string tweetDate { get; set; }
        public UsersView tweetedBy { get; set; }
        public int likeCount { get; set; }
        public string[] tags { get; set; }
        public List<RepliesView> replies { get; set; }
        public TweetsView(Tweet tweet, UsersView user, List<RepliesView> replies)
        {
            this.tweetId = tweet.Id.ToString();
            this.tweet = tweet.tweet;
            this.tweetDate = tweet.tweetDate;
            this.tweetedBy = user;
            this.likeCount = tweet.likeCount;
            this.tags = tweet.tags;
            this.replies = replies;
        }
    }
}
