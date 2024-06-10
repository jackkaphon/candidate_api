using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Dto.StockDto
{
    public class StockCreateDto
    {
        public int ProductColorId { get; set; }
        public int ProductSizeId { get; set; }
        public int Quantity { get; set; } = 0;
        [Required(ErrorMessage = "Storename is required.")]
        public decimal Price { get; set; }
    }
}
