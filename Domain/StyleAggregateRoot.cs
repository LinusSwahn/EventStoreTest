using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using Domain.Events;
using EventStore.ClientAPI;
using EventStoreRepository.Common.Aggregates;
using EventStoreRepository.Common.DomainEvents;
using EventStoreRepository.Common.Extensions;

namespace Domain
{
    public class StyleAggregateRoot : BaseAggregateRoot<StyleAggregate>
    {
        public StyleAggregateRoot()
        {
        }

        protected override void ApplyEvent(IDomainEvent domainDomain)
        {
            if (!Enum.TryParse<StyleEventType>(domainDomain.EventType, out var eventType))
            {
                return;
            }
            switch (eventType)
            {
                case StyleEventType.CreateStyle:
                    var createStyleEvent = (CreateStyle)domainDomain;
                    Id = createStyleEvent.StyleId;
                    Aggregate.Id = createStyleEvent.StyleId;
                    Aggregate.ProductData = createStyleEvent.ProductData;
                    break;
                case StyleEventType.UpdateProductData:
                    var updateProductDataEvent = (UpdateProductData)domainDomain;
                    Aggregate.ProductData = updateProductDataEvent.ProductData;
                    break;
                case StyleEventType.Create:
                    Aggregate.IsCreated = true;
                    break;
                case StyleEventType.Publish:
                    Aggregate.IsPublished = true;
                    break;
                case StyleEventType.SetCategory:
                    var setCategoryEvent = (SetCategory)domainDomain;
                    Aggregate.CategoryId = setCategoryEvent.CategoryId;
                    break;
                case StyleEventType.SetSkus:
                    var setSkusEvent = (SetSkus)domainDomain;
                    Aggregate.Variants = setSkusEvent.Variants;
                    break;
                case StyleEventType.UpdateSkuData:
                    var updateSkuDataEvent = (UpdateSkuData)domainDomain;
                    foreach (var variant in Aggregate.Variants)
                    {
                        if (variant.Value.Skus.ContainsKey(updateSkuDataEvent.Sku.Id))
                        {
                            variant.Value.Skus[updateSkuDataEvent.Sku.Id] = updateSkuDataEvent.Sku;
                        }
                    }
                    break;
                case StyleEventType.UpdateSkuPrice:
                    var updateSkuPriceEvent = (UpdateSkuPrice)domainDomain;
                    foreach (var variant in Aggregate.Variants)
                    {
                        if (variant.Value.Skus.ContainsKey(updateSkuPriceEvent.SkuId))
                        {
                            variant.Value.Skus[updateSkuPriceEvent.SkuId].Price = updateSkuPriceEvent.Price;
                        }
                    }
                    break;
                case StyleEventType.UpdateSkuStock:
                    var updateSkuStockEvent = (UpdateSkuStock)domainDomain;
                    foreach (var variant in Aggregate.Variants)
                    {
                        if (variant.Value.Skus.ContainsKey(updateSkuStockEvent.SkuId))
                        {
                            variant.Value.Skus[updateSkuStockEvent.SkuId].Stock = updateSkuStockEvent.Stock;
                        }
                    }
                    break;
            }
        }

        protected override IDomainEvent GetDomainEvent(EventData eventData)
        {
            if (!Enum.TryParse<StyleEventType>(eventData.Type, out var eventType))
            {
                throw new ArgumentOutOfRangeException();
            }
            switch (eventType)
            {
                case StyleEventType.CreateStyle:
                    return eventData.Data.GetEventData<CreateStyle>();
                case StyleEventType.UpdateProductData:
                    return eventData.Data.GetEventData<UpdateProductData>();
                case StyleEventType.Create:
                    return eventData.Data.GetEventData<Create>();
                case StyleEventType.Publish:
                    return eventData.Data.GetEventData<Publish>();
                case StyleEventType.SetCategory:
                    return eventData.Data.GetEventData<SetCategory>();
                case StyleEventType.SetSkus:
                    return eventData.Data.GetEventData<SetSkus>();
                case StyleEventType.UpdateSkuData:
                    return eventData.Data.GetEventData<UpdateSkuData>();
                case StyleEventType.UpdateSkuPrice:
                    return eventData.Data.GetEventData<UpdateSkuPrice>();
                case StyleEventType.UpdateSkuStock:
                    return eventData.Data.GetEventData<UpdateSkuStock>();
            }
            throw new ArgumentOutOfRangeException();
        }

        protected override byte[] GetMetaData()
        {
            var metadata = new StyleMetadata
            {
                Id = Id,
                Skus = Aggregate.Variants.Values.SelectMany(variant => variant.Skus.Keys).ToList()
            };
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(metadata));
        }
    }
}