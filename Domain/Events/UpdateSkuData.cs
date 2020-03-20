using EventStoreRepository.Common.DomainEvents;

namespace Domain.Events
{
    public class UpdateSkuData : IDomainEvent
    {
        public Sku Sku { get; set; } = new Sku();
        public string EventType => nameof(StyleEventType.UpdateSkuData);
    }
}