using System;
using System.Net.Http;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using HouseBot.Data.Core;
using HouseBot.Data.Services;
using Serilog;

namespace HouseBot.Client.Events
{
    public interface IEventConsumer : IDisposable
    {
        Task PollAsync();
    }

    public abstract class EventConsumer<T> : IEventConsumer where T : class, IEventData
    {
        protected readonly ILogger logger;
        private readonly IConsumer<string, T> consumer;
        private readonly GetTopicName getTopicName = new();
        private readonly TimeSpan timeoutMs;

        protected EventConsumer()
        {
            logger = Log.ForContext(GetType());

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
                GroupId = "clients",
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                MaxPollIntervalMs = 300000,
                SessionTimeoutMs = 45000,
            };

            consumer = new ConsumerBuilder<string, T>(consumerConfig)
                .SetKeyDeserializer(new JsonDeserializer<string>().AsSyncOverAsync())
                .SetValueDeserializer(new JsonDeserializer<T>().AsSyncOverAsync())
                .SetErrorHandler((_, e) => logger.Error("Failed to consume event", e))
                .Build();

            timeoutMs = TimeSpan.FromMilliseconds((double)consumerConfig.MaxPollIntervalMs - 5000);
        }

        protected abstract Task ConsumeAsync(T eventData);

        public async Task PollAsync()
        {
            try
            {
                var topicName = getTopicName.ForType<T>();
                consumer.Subscribe(topicName);

                var apiClient = new Api.Client(new HttpClient()) {BaseUrl = "https://housebot-server.conveyor.cloud"};

                int partitionIndex;
                while(true)
                {
                    try
                    {
                        partitionIndex = await apiClient.Client_GetPartitionIndexAsync(Environment.MachineName);
                        break;
                    }
                    catch
                    {
                        logger.Warning("Can't connect. Will try again soon.");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                consumer.Assign(new TopicPartition(topicName, new Partition(partitionIndex)));
                
                logger.Information("Consumer loop started...\n");

                while(true)
                {
                    try
                    {
                        var result = consumer.Consume(timeoutMs);
                        
                        var eventData = result?.Message?.Value;
                        if(eventData == null)
                        {
                            continue;
                        }

                        logger
                            .ForContext("Event.Key", result.Message.Key)
                            .ForContext("Event.Timestamp", result.Message.Timestamp.UtcDateTime.ToLocalTime())
                            .ForContext("Event.Data", eventData, true)
                            .Information("Event recieved");

                        await ConsumeAsync(result.Message.Value);

                        consumer.Commit(result);
                        consumer.StoreOffset(result);
                    }
                    catch(ConsumeException exception) when(!exception.Error.IsFatal)
                    {
                        logger.Warning(exception, "Non fatal error");
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }

        public void Dispose()
        {
            consumer?.Dispose();
        }
    }
}