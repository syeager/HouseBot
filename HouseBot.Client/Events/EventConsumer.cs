using System.Threading.Tasks;
using HouseBot.Data.Core;

namespace HouseBot.Client.Events
{
    public abstract class EventConsumer<T> where T : IEventData
    {
        public EventConsumer()
        {
            //var adminConfig = new AdminClientConfig { BootstrapServers = "127.0.0.1:9092" };
            //var schemaRegistryConfig = new SchemaRegistryConfig { Url = "http://127.0.0.1:8081" };
            //var producerConfig = new ProducerConfig
            //{
            //    BootstrapServers = "127.0.0.1:9092",
            //    EnableDeliveryReports = true,
            //    ClientId = Dns.GetHostName()
            //};

            //using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
            //using var producer = new ProducerBuilder<string, LeaveApplicationReceived>(producerConfig)
            //    .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
            //    .SetValueSerializer(new AvroSerializer<LeaveApplicationReceived>(schemaRegistry))
            //    .Build();
        }

        public abstract Task ConsumeAsync(Event<T> @event);
    }
}