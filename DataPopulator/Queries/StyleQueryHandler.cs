using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Timer;
using Domain;
using EventStoreRepository.Common;
using MediatR;
using Unit = App.Metrics.Unit;

namespace DataPopulator.Queries
{
    public class StyleQueryHandler : IRequestHandler<GetStyleByIdQuery, StyleAggregateRoot>,
        IRequestHandler<GetStyleBySkuIdQuery, StyleAggregateRoot>
    {
        private readonly IAggregateRepository _repository;
        private readonly IMetrics _metrics;
        
        private TimerOptions _requestTimer = new TimerOptions
        {
            Name = "Query timer",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        public StyleQueryHandler(IAggregateRepository repository, IMetrics metrics)
        {
            _repository = repository;
            _metrics = metrics;
        }

        public async Task<StyleAggregateRoot> Handle(GetStyleByIdQuery request, CancellationToken cancellationToken)
        {
            using (_metrics.Measure.Timer.Time(_requestTimer))
            {
                try
                {
                    return await _repository.GetByStream<StyleAggregateRoot, StyleAggregate>($"{typeof(StyleAggregateRoot).Name}-{request.StyleId}");
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public async Task<StyleAggregateRoot> Handle(GetStyleBySkuIdQuery request, CancellationToken cancellationToken)
        {
            using (_metrics.Measure.Timer.Time(_requestTimer))
            {
                try
                {
                    return await _repository.GetByStream<StyleAggregateRoot, StyleAggregate>($"sku-{request.SkuId}");
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}