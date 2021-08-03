using System;
using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using HouseBot.Data.Core;
using HouseBot.Data.Services;
using HouseBot.Server.Data;
using HouseBot.Server.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HouseBot.Server.Controllers
{
    [ApiController]
    public abstract class ConsumerController<T> : ControllerBase where T : class, IEventData
    {
        private readonly ILogger logger;
        private readonly AppData appData;
        private readonly GetTopicName getTopicName = new();

        protected ConsumerController(AppData appData)
        {
            this.appData = appData;

            logger = Log.ForContext(GetType());
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Fire(EventRequest<T> request)
        {
            var user = await appData.Users.FirstOrDefaultAsync(u => u.ApiKey == request.ApiKey);
            if(user == null)
            {
                return NotFound(request.ApiKey);
            }

            var @event = new Event<T>(Guid.NewGuid(), DateTime.UtcNow, user.Name, request.Data);
            await ProduceEventAsync(request.Target, @event);
            return Ok();
        }

        private async Task ProduceEventAsync(string target, Event<T> @event)
        {
            target = target.Sanitize();

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

            var machine = await appData.ClientMachines.FindAsync(target);
            if(machine == null)
            {
                throw new NotSupportedException(target);
            }

            var partition = new TopicPartition(
                getTopicName.ForEventData(@event.Data),
                new Partition(machine.PartitionIndex));

            var result = await producer.ProduceAsync(
                partition,
                new Message<string, T>
                {
                    Key = @event.Id.ToString(),
                    Value = @event.Data
                });

            logger
                .ForContext("Topic", result.Topic)
                .ForContext("Partition", result.Partition.Value)
                .Information("Event produced");
        }
    }
}