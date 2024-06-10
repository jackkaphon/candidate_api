using EcommerceApi.Dto.StockDto;
using EcommerceApi.Entities;
using System.Text.Json.Serialization;

namespace EcommerceApi.Dto.ProductDto
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<StockResponseDto> Stocks { get; set; }
        
    }
}
