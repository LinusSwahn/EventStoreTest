using Domain;
using MediatR;

namespace DataPopulator.Queries
{
    public class GetStyleByIdQuery : IRequest<StyleAggregateRoot>
    {
        public string StyleId { get; set; }
    }
}