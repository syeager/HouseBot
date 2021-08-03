using System;
using System.Threading;
using System.Threading.Tasks;
using HouseBot.Client.Consumers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HouseBot.Client
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly WindowsNotificationConsumer consumer = new();

        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerTask = consumer.PollAsync();

            while(!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.WhenAny(
                    consumerTask,
                    Task.Delay(1000, stoppingToken));
            }
        }
    }
}