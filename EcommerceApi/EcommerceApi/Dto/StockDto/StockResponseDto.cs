using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Dto.StockDto
{
    public class StockResponseDto
    {
        public int Color { get; set; }
        public int Size { get; set; }
        public int Quantity { get; set; } 
        public decimal Price { get; set; }
    }
}
