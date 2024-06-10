using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Dto.CartDto
{
    public class CartItemDto
    {
        [Required(ErrorMessage = "ProductId is required.")]
        public required int StockId { get; set; }
        [Required(ErrorMessage = "Quatity is required.")]
        public required int Quatity { get; set; }
    }
}
