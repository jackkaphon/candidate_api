using System.Text.Json.Serialization;

namespace EcommerceApi.Entities
{
    public class Cart:BaseEntity
    {
        public int UserId { get; set; }
        [JsonIgnore]
        public List<CartItem> CartItems { get; set; }
    }
}
