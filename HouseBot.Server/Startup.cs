using System.Reflection;
using HouseBot.Server.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HouseBot.Server
{
    public class Startup
    {
        private readonly string version;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            version = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "HouseBot";
                settings.Version = version;
            });

            services.AddDbContext<AppData>(options => options.UseSqlite("Filename=HouseBotDatabase.db"));

            InitDb(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void InitDb(IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            using var appData = serviceProvider.GetRequiredService<AppData>();
            appData.Database.EnsureCreated();

            //var creationScript = appData.Database.GenerateCreateScript();

            //try
            //{
            //    appData.Database.ExecuteSqlRaw(creationScript);
            //}
            //catch(SqliteException exception)
            //{
            //    Console.WriteLine(exception);
            //}
        }
    }
}