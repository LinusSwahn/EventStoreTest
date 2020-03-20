using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Timer;
using Domain;
using Domain.Events;
using EventStoreRepository.Common;
using EventStoreRepository.Common.Aggregates;
using MediatR;
using Unit = App.Metrics.Unit;

namespace DataPopulator.Commands
{
    public class StyleCommandHandler : IRequestHandler<CreateStyleCommand>
    {
        private readonly IAggregateFactory _factory;
        private readonly IAggregateRepository _repository;
        private readonly IMetrics _metrics;

        private TimerOptions _commandTimer = new TimerOptions
        {
            Name = "Command timer",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        public StyleCommandHandler(IAggregateFactory factory, IAggregateRepository repository, IMetrics metrics)
        {
            _factory = factory;
            _repository = repository;
            _metrics = metrics;
        }

        public async Task<MediatR.Unit> Handle(CreateStyleCommand request, CancellationToken cancellationToken)
        {
            var style = _factory.Create<IAggregateRoot<StyleAggregate>, StyleAggregateRoot>();

            var domainEvent = new CreateStyle
            {
                StyleId = request.StyleId,
                ProductData = request.ProductData
            };

            style.AddEvent(domainEvent);
            using (_metrics.Measure.Timer.Time(_commandTimer))
            {
                await _repository.SaveAsync(style, $"{typeof(StyleAggregateRoot).Name}-{request.StyleId}");
            }

            return MediatR.Unit.Value;
        }
    }
}