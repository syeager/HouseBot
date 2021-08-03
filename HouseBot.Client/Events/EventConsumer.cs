using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using HouseBot.Data.Core;
using HouseBot.Data.Services;

namespace HouseBot.Client.Events
{
    public abstract class EventConsumer<T> : IDisposable where T : class, IEventData
    {
        private readonly IConsumer<string, T> consumer;
        private readonly GetPartitionIndex getPartitionIndex = new();
        private readonly GetTopicName getTopicName = new();
        private readonly TimeSpan timeoutMs;

        protected EventConsumer()
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
                GroupId = "clients",
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false,
                // Read messages from start if no commit exists.
                AutoOffsetReset = AutoOffsetReset.Earliest,
                MaxPollIntervalMs = 300000,
                SessionTimeoutMs = 45000,
            };

            consumer = new ConsumerBuilder<string, T>(consumerConfig)
                .SetKeyDeserializer(new JsonDeserializer<string>().AsSyncOverAsync())
                .SetValueDeserializer(new JsonDeserializer<T>().AsSyncOverAsync())
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
                var partitionIndex = await getPartitionIndex.ForMachineNameAsync(Environment.MachineName);
                consumer.Assign(new TopicPartition(topicName, new Partition(partitionIndex)));
                Console.WriteLine("Consumer loop started...\n");

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

                        await ConsumeAsync(result.Message.Value);

                        consumer.Commit(result);
                        consumer.StoreOffset(result);
                    }
                    catch(ConsumeException e) when(!e.Error.IsFatal)
                    {
                        Console.WriteLine($"Non fatal error: {e}");
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