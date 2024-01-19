using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.Models
{
    public class Login
    {
        public string loginId { get; set; }
        public string password { get; set; }
        public Login(string loginId, string password)
        {
            this.loginId = loginId;
            this.password = password;
        }
    }
}
