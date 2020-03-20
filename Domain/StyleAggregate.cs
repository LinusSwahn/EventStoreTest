using System.Collections.Generic;

namespace Domain
{
    public class StyleAggregate
    {
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public ProductData ProductData { get; set; } = new ProductData(); 
        public Dictionary<string, Variant> Variants { get; set; } = new Dictionary<string, Variant>();
        public bool IsCreated { get; set; }
        public bool IsPublished { get; set; }
    }

    public class ProductData
    {
        public string Name { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
    }

    public class Variant
    {
        public string Id { get; set; }
        public VariantData VariantData { get; set; } = new VariantData();
        public Dictionary<string, Sku> Skus { get; set; } = new Dictionary<string, Sku>();
    }

    public class VariantData
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class Sku
    {
        public string Id { get; set; }
        public SkuData SkuData { get; set;  } = new SkuData();
        public int Stock { get; set; }
        public int Price { get; set; }
    }

    public class SkuData
    {
        public string Name { get; set; }
        public string Size { get; set; }
    }
}