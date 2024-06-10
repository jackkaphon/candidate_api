using EcommerceApi.Dto.UserDto;
using EcommerceApi.Entities;
using EcommerceApi.Services.Implementation;
using EcommerceApi.Services.Interface;
using EcommerceApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EcommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
  
     
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto request)
        {
            //var cart = new Cart { UserId = newUser.UserId };
            //await _productService.AddCart(cart);
            return Ok(await _userService.CreateUser(request));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            //var cart = await _productService.GetCartByUserId(user.UserId);

            //// Return the user data along with the cartId for the session
            //return Ok(new { User = user, CartId = cart.Id });
            return Ok(await _userService.Login(request));
        }


    }
}
