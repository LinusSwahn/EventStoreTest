using System.Collections.Generic;

namespace Domain.Events
{
    public class StyleMetadata
    {
        public string Id { get; set; }
        public List<string> Skus { get; set; } = new List<string>();
    }
}