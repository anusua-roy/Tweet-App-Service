using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace com.tweetapp.Kafka
{
    public class Producer : IProducer
    {
        private readonly ProducerConfig config;
        private readonly ILogger<Producer> logger;
        public Producer(IConfiguration config, ILogger<Producer> logger)
        {
            this.config = new ProducerConfig
            {
                BootstrapServers = config.GetConnectionString("BootstrapServers"),
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = config.GetConnectionString("Username"),
                SaslPassword = config.GetConnectionString("Password")
            };
            this.logger = logger;
        }
        public async Task<bool> SendRequestToKafkaAsync(string topic, string message)
        {

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                try
                {
                    var result = await producer.ProduceAsync(topic, new Message<string, string> { Key = topic, Value = message });
                    return true;
                }
                catch (ProduceException<string, string> e)
                {
                    logger.LogError(e.Error.Reason, e);
                }
            }
            return false;
        }
    }
}
