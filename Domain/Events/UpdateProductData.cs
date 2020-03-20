using EventStoreRepository.Common.DomainEvents;

namespace Domain.Events
{
    public class UpdateProductData : IDomainEvent
    {
        public ProductData ProductData { get; set; } = new ProductData();
        public string EventType => nameof(StyleEventType.UpdateProductData);
    }
}