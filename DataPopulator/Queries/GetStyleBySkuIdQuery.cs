using Domain;
using MediatR;

namespace DataPopulator.Queries
{
    public class GetStyleBySkuIdQuery : IRequest<StyleAggregateRoot>
    {
        public string SkuId { get; set; }
    }
}