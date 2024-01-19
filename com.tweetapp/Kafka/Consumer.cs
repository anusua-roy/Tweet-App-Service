using com.tweetapp.MongoRepository;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace com.tweetapp.Kafka
{
    public class Consumer : BackgroundService
    {
        private readonly ConsumerConfig config;
        private readonly ILogger<Consumer> logger;
        private readonly IDbRequest dbRequest;

        public Consumer(IConfiguration config, ILogger<Consumer> logger, IDbRequest dbRequest)
        {
            this.config = new ConsumerConfig
            {
                GroupId = "webapi-integration",
                BootstrapServers = config.GetConnectionString("BootstrapServers"),
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = config.GetConnectionString("Username"),
                SaslPassword = config.GetConnectionString("Password"),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            this.logger = logger;
            this.dbRequest = dbRequest;

        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using (var builder = new ConsumerBuilder<string, string>(config).Build())
            {

                builder.Subscribe(Global.REQUEST_TYPES);
                try
                {
                    await Task.Run(async () =>
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            var consumer = builder.Consume(cancellationToken);
                            if (!await dbRequest.processRequest(consumer.Message.Key, consumer.Message.Value))
                                logger.LogError("Message was not inserted");
                            else
                                logger.LogInformation("Message Processed");

                        }
                    }, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                    builder.Close();
                }
            }
        }
    }
}
