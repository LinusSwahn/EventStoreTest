using Domain;
using MediatR;

namespace DataPopulator.Commands
{
    public class CreateStyleCommand : IRequest
    {
        public string StyleId { get; set; }
        public ProductData ProductData { get; set; }
    }
}