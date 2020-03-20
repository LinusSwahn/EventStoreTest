using System;
using System.Net;
using System.Reflection;
using App.Metrics;
using App.Metrics.Formatters.Json;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Projections;
using EventStoreRepository.Common;
using EventStoreRepository.Common.Extensions;
using EventStoreRepository.Common.Settings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = EventStore.ClientAPI.ILogger;

namespace DataPopulator
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<EventStoreSettings>(hostContext.Configuration.GetSection("EventStoreSettings"));

                    services.AddLogging(builder =>
                    {
                        builder.SetMinimumLevel(LogLevel.Debug);
                        builder.AddConsole();
                    });

                    services.AddSingleton<ILogger, SeriLogger>();

                    services.AddSingleton(provider =>
                    {
                        var builder = ConnectionSettings.Create();
                        builder.FailOnNoServerResponse();
                        builder.KeepReconnecting();
                        builder.KeepRetrying();
                        var settings = builder.Build();
                        return EventStoreConnection.Create(settings, new Uri(provider
                            .GetService<IOptionsMonitor<EventStoreSettings>>()
                            .CurrentValue.ConnectionString));
                    });
                    services.AddSingleton(provider =>
                    {
                        var logger = provider.GetService<ILogger>();
                        return new ProjectionsManager(logger, new DnsEndPoint("localhost", 2113),
                            TimeSpan.FromSeconds(15));
                    });

                    services.AddMetrics(builder =>
                    {
                        builder.Report.ToConsole(options =>
                        {
                            options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                        }).Build();
                    });

                    services.AddMediatR(Assembly.GetExecutingAssembly());

                    services.AddEventStoreRepository();
                    
                    services.AddHostedService<PopulationService>();
                });
    }
}