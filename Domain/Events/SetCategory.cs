using EventStoreRepository.Common.DomainEvents;

namespace Domain.Events
{
    public class SetCategory : IDomainEvent
    {
        public string CategoryId { get; set; }
        public string EventType => nameof(StyleEventType.SetCategory);
    }
}