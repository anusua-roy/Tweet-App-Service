using System.Threading.Tasks;

namespace com.tweetapp.MongoRepository
{
    public interface IDbRequest
    {
        Task<bool> processRequest(string requestType, string data);
    }
}
