using EcommerceApi.Dto.CartDto;
using EcommerceApi.Dto.ProductDto;
using EcommerceApi.Entities;
using EcommerceApi.Utils;

namespace EcommerceApi.Services.Interface
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllProducts(int storeId);
        Task<ProductResponseDto> GetProductById(int productId);
        Task<MessageResponse> CreateProduct(int storeId, ProductCreateDto request);
        Task UpdateProduct(int storeId, ProductUpdateDto request);
        Task UpdateProductPrice(Stock request);
        Task DeleteProduct(int productId);
        Task<Cart> GetCartByUserId(int userId);
        Task<CartItem> AddItemToCart(int cartId, CartItemDto cartItemDto);
        Task RemoveItemFromCart(int cartItemId);
    }
}
