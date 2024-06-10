using EcommerceApi.Data;
using EcommerceApi.Dto.CartDto;
using EcommerceApi.Dto.ProductDto;
using EcommerceApi.Dto.StockDto;
using EcommerceApi.Dto.StoreDto;
using EcommerceApi.Dto.UserDto;
using EcommerceApi.Entities;
using EcommerceApi.Exceptions;
using EcommerceApi.Services.Interface;
using EcommerceApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcommerceApi.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dbContext;
        private readonly ILogger<ProductService> _logger;
        public ProductService(DataContext context, ILogger<ProductService> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public async Task<List<ProductResponseDto>> GetAllProducts(int storeId)
        {

            var result = await _dbContext.Products.Where(p => !p.IsDeleted && p.StoreId == storeId)
    .Select(u => new ProductResponseDto
    {
        Id = u.Id,
        Name = u.Name,
        Stocks = u.Stokcs.Select(s => new StockResponseDto
        {
            Color=s.ProductColorId,
            Size=s.ProductSizeId,
        }).ToList()
    })
    .ToListAsync();

            if (result == null)
            {
                return new List<ProductResponseDto>();
            }
            return result;
        }

        public async Task<ProductResponseDto> GetProductById(int productId)
        {
            var result = await _dbContext.Products.Where(p => !p.IsDeleted && p.Id == productId)
     .Select(u => new ProductResponseDto
     {
         Id = u.Id,
         Name = u.Name,
         Stocks = u.Stokcs.Select(s => new StockResponseDto
         {
             Color = s.ProductColorId,
             Size = s.ProductSizeId,
         }).ToList()
     })
     .FirstOrDefaultAsync();

            if (result == null)
            {
                return new ProductResponseDto();
            }
            return result;
        }

        public async Task<MessageResponse> CreateProduct(int storeId, ProductCreateDto request)
        {
            var result = new MessageResponse()
            {
                IsSuccessful = false,
                Messages = new List<string>(),
                Contents = new Dictionary<string, object>()
            };
            using (var dbTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var product = new Product { Name = request.Name, CategoryId = request.CategoryId, StoreId = storeId };
                    _dbContext.Products.Add(product);
                    await _dbContext.SaveChangesAsync();

                    if (request.stocks != null && request.stocks.Count > 0)
                    {
                        foreach (var stock in request.stocks)
                        {
                            _dbContext.Stock.Add(new Stock
                            {
                                ProductId = product.Id,
                                Quantity = stock.Quantity,
                                Price = stock.Price,
                                ProductColorId = stock.ProductColorId,
                                ProductSizeId = stock.ProductSizeId
                            });
                        }
                    }
                    else
                    {
                       var stock=(new Stock { ProductId = product.Id, Quantity = 0, Price = 0,ProductSizeId=1,ProductColorId=1 });
                        _dbContext.Stock.Add(stock);
                    }
                    await _dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    result.Id = product.Id;
                    result.IsSuccessful = true;
                    result.Messages.Add("Successfully created.");
                }
                catch (DbUpdateException dbEx)
                {
                    await dbTransaction.RollbackAsync();
                    _logger.LogError(dbEx, "An error occurred while saving changes to the database.");
                    throw new HttpResponseException(StatusCodes.Status500InternalServerError, "An error occurred while creating the user. Please try again later.");
                }
                catch (Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    _logger.LogError(ex, "An unexpected error occurred.");
                    throw new HttpResponseException(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
                }
            }
                return result;
            }

            public async Task UpdateProduct(int productId,ProductUpdateDto request)
            {
            Product productDb = _dbContext.Products.Where(p => p.Id == productId & !p.IsDeleted).FirstOrDefault();
            if (productDb == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Product not exists");
            }
            productDb.Name = request.Name;
            _dbContext.Entry(productDb).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
           
              
            }

            public async Task DeleteProduct(int productId)
            {
                var product = await _dbContext.Set<Product>().FindAsync(productId);
                if (product != null)
                {
                    _dbContext.Set<Product>().Remove(product);
                    await _dbContext.SaveChangesAsync();
                }
            }

            public async Task<Cart> GetCartByUserId(int userId)
            {
                return await _dbContext.Set<Cart>().FirstOrDefaultAsync(c => c.UserId == userId);
            }

            public async Task<CartItem> AddItemToCart(int cartId, CartItemDto cartItemDto)
            {
                var cart = await _dbContext.Carts.FindAsync(cartId);
                var product = await _dbContext.Stock.FindAsync(cartItemDto.StockId);
                var cartItem = new CartItem
                {
                    CartId = cartId,
                    StockId = cartItemDto.StockId,
                    Quantity = cartItemDto.Quatity
                };

                _dbContext.CartItems.Add(cartItem);
                await _dbContext.SaveChangesAsync();

                return cartItem;
            }

            public async Task RemoveItemFromCart(int cartItemId)
            {
                var cartItem = await _dbContext.Set<CartItem>().FindAsync(cartItemId);

                if (cartItem != null)
                {
                    _dbContext.CartItems.Remove(cartItem);
                    await _dbContext.SaveChangesAsync();
                }
            }

        public Task UpdateProductPrice(Stock request)
        {
            throw new NotImplementedException();
        }

    
    }
    }
