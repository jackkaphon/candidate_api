using EcommerceApi.Dto.StockDto;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Dto.ProductDto
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Productname is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        public int CategoryId { get; set; }

    

       public List<StockCreateDto> stocks { get; set; }
    }
}
