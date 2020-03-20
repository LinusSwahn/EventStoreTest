using System.Collections.Generic;
using EventStoreRepository.Common.DomainEvents;

namespace Domain.Events
{
    public class SetSkus : IDomainEvent
    {
        public Dictionary<string, Variant> Variants { get; set; }
        public string EventType => nameof(StyleEventType.SetSkus);
    }
}