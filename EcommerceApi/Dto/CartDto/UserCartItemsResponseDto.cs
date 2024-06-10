namespace EcommerceApi.Dto.CartDto
{
    public class UserCartItemsResponseDto
    {
        public int StockId { get; set; }
        public string ProductDetails { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
