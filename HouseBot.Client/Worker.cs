using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HouseBot.Client.Consumers;
using HouseBot.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HouseBot.Client
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IServiceProvider services;

        public Worker(ILogger<Worker> logger, IServiceProvider services)
        {
            this.logger = logger;
            this.services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumers = CreateConsumers();
            foreach(var consumer in consumers)
            {
                consumer.PollAsync();
            }

            while(!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private IReadOnlyCollection<IEventConsumer> CreateConsumers()
        {
            var consumerTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IEventConsumer)) && !t.IsAbstract);

            var consumers = consumerTypes
                .Select(t => (IEventConsumer)services.GetRequiredService(t))
                .ToArray();

            return consumers;
        }
    }
}