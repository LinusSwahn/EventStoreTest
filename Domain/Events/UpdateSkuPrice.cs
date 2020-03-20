using EventStoreRepository.Common.DomainEvents;

namespace Domain.Events
{
    public class UpdateSkuPrice : IDomainEvent
    {
        public string SkuId { get; set; }
        public int Price { get; set; }
        public string EventType => nameof(StyleEventType.UpdateSkuPrice);
    }
}