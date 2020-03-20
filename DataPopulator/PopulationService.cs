using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using DataPopulator.Commands;
using DataPopulator.Queries;
using Domain;
using Domain.Events;
using EventStore.ClientAPI;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataPopulator
{
    public class PopulationService : IHostedService
    {
        private readonly IMediator _mediator;
        private readonly IEventStoreConnection _connection;
        private readonly IMetricsRoot _metrics;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<PopulationService> _logger;

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public PopulationService(IMediator mediator, IEventStoreConnection connection, IMetricsRoot metrics, 
            IHostApplicationLifetime lifetime, ILogger<PopulationService> logger)
        {
            _mediator = mediator;
            _connection = connection;
            _metrics = metrics;
            _lifetime = lifetime;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => RunAsync(_tokenSource.Token), cancellationToken);
            Task.Run(() => ReportMetrics(_tokenSource.Token), cancellationToken);
            return Task.CompletedTask;
        }

        private async Task ReportMetrics(CancellationToken token)
        {
            while (!_tokenSource.Token.IsCancellationRequested)
            {
                await Task.WhenAll(_metrics.ReportRunner.RunAllAsync());
                await Task.Delay(TimeSpan.FromSeconds(5), token);
            }
        }

        private async Task RunAsync(CancellationToken token)
        {
            var random = new Random();
            var connected = false;
            _connection.Connected += (sender, args) => connected = true; 
            await _connection.ConnectAsync();
            while (!connected)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), token);
            }
            _logger.LogInformation("Connected to eventstore.");

            for (int i = 0; i < 10000; i++)
            {
                if (token.IsCancellationRequested)
                {
                    continue;
                }

                var style = await _mediator.Send(new GetStyleByIdQuery {StyleId = i.ToString()}, token);
                if (style == null)
                {
                    await _mediator.Send(new CreateStyleCommand
                    {
                        StyleId = i.ToString(),
                        ProductData = new ProductData
                        {
                            Height = random.Next(),
                            Name = Guid.NewGuid().ToString(),
                            Weight = random.Next()
                        }
                    }, token);
                }
            }
            
            _logger.LogInformation("Data population finished, stopping application.");
            _lifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}