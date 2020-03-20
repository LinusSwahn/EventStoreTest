using EventStoreRepository.Common.DomainEvents;

namespace Domain.Events
{
    public class UpdateSkuStock : IDomainEvent
    {
        public string SkuId { get; set; }
        public int Stock { get; set; }
        public string EventType => nameof(StyleEventType.UpdateSkuStock);
    }
}