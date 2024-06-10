using System.Text.Json.Serialization;

namespace EcommerceApi.Entities
{
    public class OrderItem:BaseEntity
    {
        public int StockId { get; set; }
        [JsonIgnore]
        public Stock Product { get; set; }

        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }
        
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
