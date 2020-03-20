using EventStoreRepository.Common.DomainEvents;

namespace Domain.Events
{
    public class Create : IDomainEvent
    {
        public string EventType => nameof(StyleEventType.Create);
    }
}