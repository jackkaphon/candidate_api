using EcommerceApi.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceApi.Dto.StoreDto
{
    public class StoreCreateDto
    {
        [Required(ErrorMessage = "Storename is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "OwnerId is required.")]
        public int OwnerId { get; set; }
    }
}
