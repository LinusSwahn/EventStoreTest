using System;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Projections;
using EventStoreRepository.Common;
using EventStoreRepository.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = EventStore.ClientAPI.ILogger;

namespace EventStoreTest
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
                        return new ProjectionsManager(logger, new DnsEndPoint("localhost", 2113), TimeSpan.FromSeconds(15));
                    });

                    services.AddHostedService<EventStoreHostedService>();
                });
    }
}