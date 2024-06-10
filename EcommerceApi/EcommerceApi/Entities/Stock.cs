using System.Text.Json.Serialization;

namespace EcommerceApi.Entities
{
 
        public class Stock : BaseEntity
        {
            public int ProductId { get; set; }
            public int ProductColorId { get; set; }
            public int ProductSizeId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        [JsonIgnore]
            public ProductColor Color { get; set; }
        [JsonIgnore]
        public ProductSize Size { get; set; }
    }
    
}
