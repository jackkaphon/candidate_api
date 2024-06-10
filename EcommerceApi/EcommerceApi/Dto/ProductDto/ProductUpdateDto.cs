using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Dto.ProductDto
{
    public class ProductUpdateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Name { get; set; }
    }
}
