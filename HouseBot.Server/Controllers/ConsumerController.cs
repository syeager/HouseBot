using System;
using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using HouseBot.Data.Core;
using HouseBot.Data.Services;
using HouseBot.Server.Authorization;
using HouseBot.Server.Events;
using Microsoft.AspNetCore.Mvc;

namespace HouseBot.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ConsumerController<T> : ControllerBase where T : class, IEventData
    {
        private readonly ApiKeyStore apiKeyStore;
        private readonly GetPartitionIndex getPartitionIndex;
        private readonly GetTopicName getTopicName = new();

        protected ConsumerController(ApiKeyStore apiKeyStore, GetPartitionIndex getPartitionIndex)
        {
            this.apiKeyStore = apiKeyStore;
            this.getPartitionIndex = getPartitionIndex;
        }

        [HttpPost]
        public async Task<IActionResult> Fire(EventRequest<T> request)
        {
            var user = apiKeyStore.GetUser(request.ApiKey);
            var @event = new Event<T>(Guid.NewGuid(), DateTime.UtcNow, user, request.Data);
            await ProduceEventAsync(request.Target, @event);
            return Ok();
        }

        private async Task ProduceEventAsync(string target, Event<T> @event)
        {
            var schemaRegistryConfig = new SchemaRegistryConfig {Url = "http://127.0.0.1:8081"};
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
                EnableDeliveryReports = true,
                ClientId = Dns.GetHostName()
            };

            using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
            using var producer = new ProducerBuilder<string, T>(producerConfig)
                .SetKeySerializer(new JsonSerializer<string>(schemaRegistry))
                .SetValueSerializer(new JsonSerializer<T>(schemaRegistry))
                .Build();

            var partitionIndex = await getPartitionIndex.ForMachineNameAsync(target);

            var partition = new TopicPartition(
                getTopicName.ForEventData(@event.Data),
                new Partition(partitionIndex));

            var result = await producer.ProduceAsync(
                partition,
                new Message<string, T>
                {
                    Key = @event.Id.ToString(),
                    Value = @event.Data
                });

            Console.WriteLine(
                $"\nMsg: Your leave request is queued at offset {result.Offset.Value} in the Topic {result.Topic}:{result.Partition.Value}\n\n");
        }
    }
}