using System.Text.Json.Serialization;
using static EcommerceApi.Enum.Enum;

namespace EcommerceApi.Entities
{
    public class Order:BaseEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus Status { get; set; }
        [JsonIgnore]
        public List<OrderItem> OrderItems { get; set; }
    }
}
