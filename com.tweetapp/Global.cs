using System.Collections.Generic;

namespace com.tweetapp
{
    public class Global
    {
        public static readonly List<string> REQUEST_TYPES = new List<string> {
            "REGISTER_USER", "CREATE_TWEET", "UPDATE_TWEET", "DELETE_TWEET", "LIKE_TWEET", "REPLY_TWEET"
        };
    }
}
