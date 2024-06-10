using static EcommerceApi.Enum.Enum;

namespace EcommerceApi.Dto.CartDto
{
    public class UserCartResponseDto
    {
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public List<UserCartItemsResponseDto> Items { get; set; }
    }
}
