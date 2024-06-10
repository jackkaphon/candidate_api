using System.Text.Json.Serialization;

namespace EcommerceApi.Entities
{
    public class StockInfoHistory
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        [JsonIgnore]
        public Stock Stock { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
    }
}
