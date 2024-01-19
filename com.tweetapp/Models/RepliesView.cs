namespace com.tweetapp.Models
{
    public class RepliesView
    {
        public string replyId { get; set; }
        public string reply { get; set; }
        public string replyDate { get; set; }
        public UsersView repliedBy { get; set; }
        public string[] tags { get; set; }

        public RepliesView(Reply reply, UsersView user)
        {
            this.replyId = reply.Id.ToString();
            this.reply = reply.reply;
            this.replyDate = reply.replyDate;
            this.repliedBy = user;
            this.tags = reply.tags;
        }
    }
}
