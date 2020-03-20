using EventStoreRepository.Common.DomainEvents;

namespace Domain.Events
{
    public class CreateStyle : IDomainEvent
    {
        public string StyleId { get; set; }
        public ProductData ProductData { get; set; } = new ProductData();

        public string EventType => nameof(StyleEventType.CreateStyle);
    }
}