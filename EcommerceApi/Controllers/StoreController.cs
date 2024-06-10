using EcommerceApi.Dto.ProductDto;
using EcommerceApi.Dto.StoreDto;
using EcommerceApi.Entities;
using EcommerceApi.Exceptions;
using EcommerceApi.Services.Implementation;
using EcommerceApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EcommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        public StoreController(IStoreService storeService, IUserService userService, IProductService productService)
        {
            _storeService = storeService;
            _userService = userService;
            _productService = productService;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetAllStores()
        {
            int ownerId = int.Parse(_userService.GetUserInfo().UserId);
            var stores = await _storeService.GetAllStoresAsync(ownerId);
            return Ok(stores);
        }

        [HttpGet("{storeId}"), Authorize]
        public async Task<IActionResult> GetStoreById(int storeId)
        {
            checkPermission(storeId);
            var store = await _storeService.GetStoreByIdAsync(storeId);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddStore([FromBody] StoreCreateDto request)
        {
            request.OwnerId = int.Parse(_userService.GetUserInfo().UserId);
            return Ok(await _storeService.AddStoreAsync(request));
        }



        [HttpPost("AddManager/{storeId}/{userId}"), Authorize]
        public async Task<IActionResult> AddManager(int storeId, int userId)
        {
            checkPermission(storeId);
            return Ok(await _storeService.AddManager(storeId, userId));
        }
        [HttpPost("RevorkManager/{storeId}/{userId}"), Authorize]
        public async Task<IActionResult> RevorkManager(int storeId, int userId)
        {
            checkPermission(storeId);
            return Ok(await _storeService.Revork(storeId, userId));
        }

        [HttpPut("{storeId}"), Authorize]
        public async Task<IActionResult> UpdateStore(int storeId, [FromBody] StoreUpdateDto request)
        {
            checkPermission(storeId);
            return Ok(await _storeService.UpdateStoreAsync(storeId, request));
        }

        [HttpDelete("{storeId}"), Authorize]
        public async Task<IActionResult> DeleteStore(int storeId)
        {
            checkPermission(storeId);
            var existingStore = await _storeService.GetStoreByIdAsync(storeId);
            if (existingStore == null)
            {
                return NotFound();
            }
            await _storeService.DeleteStoreAsync(storeId);
            return NoContent();
        }




        [HttpPost("{storeId}/Product"), Authorize]
        public async Task<IActionResult> CreateProduct( int storeId,[FromBody] ProductCreateDto request)
        {
            checkPermission(storeId);
            var newProduct = await _productService.CreateProduct(storeId, request);
            return Ok(newProduct);
        }


        [HttpGet("{storeId}/Products"), Authorize]
        public async Task<IActionResult> GetAllProducts(int storeId)
        {
            checkPermission(storeId);
            var products = await _productService.GetAllProducts(storeId);
            return Ok(products);
        }

        [HttpGet("{storeId}/Product/{productId}"), Authorize]
        public async Task<IActionResult> GetProductById(int storeId,int productId)
        {
            checkPermission(storeId);
            var product = await _productService.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPut("{storeId}/Product/{productId}"), Authorize]
        public async Task<IActionResult> UpdateProduct(int storeId, int productId, [FromBody] ProductUpdateDto product)
        {           
            checkPermission(storeId);
            await _productService.UpdateProduct(productId,product);
            return NoContent();
        }



        private void checkPermission(int storeId)
        {
            var userInfo = _userService.GetUserInfo();
            if (userInfo.OwnerOfStores != null)
            {
                foreach (var item in userInfo.OwnerOfStores)
                {
                    if (int.Parse(item.Value) == storeId)
                    {
                        return;
                    }
                }
            }
            if (userInfo.ManagerOfStores != null)
            {
                foreach (var item in userInfo.ManagerOfStores)
                {
                    if (int.Parse(item.Value) == storeId)
                    {
                        return;
                    }
                }
            }

            throw new HttpResponseException(StatusCodes.Status401Unauthorized, "You don't have permission to manage this store");

        }
    }
}
