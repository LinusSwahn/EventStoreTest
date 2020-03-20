using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventStoreTest
{
    public class EventStoreHostedService : IHostedService
    {
        private readonly ProjectionsManager _manager;
        private readonly IEventStoreConnection _connection;
        private readonly ILogger<EventStoreHostedService> _logger;

        private bool _isConnected;

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public EventStoreHostedService(ProjectionsManager manager, IEventStoreConnection connection,
            ILogger<EventStoreHostedService> logger)
        {
            _manager = manager;
            _connection = connection;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(RunAsync);
            return Task.CompletedTask;
        }

        private async Task RunAsync()
        {
            var token = _tokenSource.Token;
            _connection.Connected += ConnectionOnConnected;
            _connection.Disconnected += ConnectionOnDisconnected;
            _connection.ErrorOccurred += ConnectionOnErrorOccurred;
            await _connection.ConnectAsync();


            while (!_isConnected && !token.IsCancellationRequested)
            {
                await Task.Delay(25, token);
            }

            try
            {
                var credentials = new UserCredentials("admin", "changeit");

                var projections = await _manager.ListContinuousAsync(credentials);

                if (projections.Exists(details => details.Name == "EmailIndexProjection"))
                {
                    await _manager.UpdateQueryAsync(
                        "EmailIndexProjection",
                        @"fromCategory('AccountAggregate')
                                .when({
                                    $any: function(s, e){
                                        linkTo(e.metadata.PrimaryEmail, e)
                                    }
                                })",
                        true,
                        credentials);
                }
                else
                {
                    await _manager.CreateContinuousAsync(
                        "EmailIndexProjection",
                        @"fromCategory('AccountAggregate')
                                .when({
                                    $any: function(s, e){
                                        linkTo(e.metadata.PrimaryEmail, e)
                                    }
                                })",
                        true,
                        credentials);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }

        private void ConnectionOnConnected(object? sender, ClientConnectionEventArgs e)
        {
            _isConnected = true;
            _logger.LogInformation("Connected to EventStore.");
        }

        private void ConnectionOnDisconnected(object? sender, ClientConnectionEventArgs e)
        {
            _logger.LogInformation("Disconnected from EventStore.");
        }

        private void ConnectionOnErrorOccurred(object? sender, ClientErrorEventArgs e)
        {
            _logger.LogCritical(e.Exception, "Error from EventStore.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}