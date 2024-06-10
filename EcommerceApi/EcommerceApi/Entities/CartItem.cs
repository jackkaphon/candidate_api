using System.Text.Json.Serialization;

namespace EcommerceApi.Entities
{
    public class CartItem:BaseEntity
    {
        public int StockId { get; set; }
        [JsonIgnore]
        public Stock Product { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int Quantity { get; set; }
    }
}
