using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using HouseBot.Data.Core;
using HouseBot.Data.Services;
using HouseBot.Server.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;

namespace HouseBot.Server
{
    public class Startup
    {
        private readonly ILogger logger = Log.ForContext<Startup>();
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

            InitJson(services, version);
            InitDb(services);
            InitTopics().Wait();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseOpenApi()
                .UseSwaggerUi3();

            app
                .UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void InitJson(IServiceCollection services, string version)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new StringEnumConverter());

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.Converters = serializerSettings.Converters);
            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "HouseBot";
                settings.Version = version;
                settings.SerializerSettings = serializerSettings;
            });
        }

        private void InitDb(IServiceCollection services)
        {
            services.AddDbContext<AppData>(options => options.UseSqlite(configuration.GetConnectionString("AppData")));

            using var serviceProvider = services.BuildServiceProvider();
            using var appData = serviceProvider.GetRequiredService<AppData>();
            appData.Database.EnsureCreated();

            var creationScript = appData.Database.GenerateCreateScript();

            try
            {
                appData.Database.ExecuteSqlRaw(creationScript);
            }
            catch (Exception exception)
            {
                logger.Information(exception, "Could not run SQL DB creation script");
            }
        }

        private async Task InitTopics()
        {
            var topics = typeof(IEventData).Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IEventData)) && !t.IsAbstract)
                .Select(t => new GetTopicName().ForType(t))
                .Select(t => new TopicSpecification {Name = t, ReplicationFactor = 1, NumPartitions = 10});

            using var adminClient = new AdminClientBuilder(new AdminClientConfig {BootstrapServers = "127.0.0.1:9092"}).Build();
            try
            {
                await adminClient.CreateTopicsAsync(topics);
            }
            catch(Exception exception)
            {
                logger.Information(exception, "An error occured creating topic");
            }
        }
    }
}