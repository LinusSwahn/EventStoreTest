using EventStoreRepository.Common.DomainEvents;

namespace Domain.Events
{
    public class Publish : IDomainEvent
    {
        public string EventType => nameof(StyleEventType.Publish);
    }
}