using EcommerceApi.Dto.CartDto;
using EcommerceApi.Dto.UserDto;
using EcommerceApi.Entities;
using EcommerceApi.Utils;

namespace EcommerceApi.Services.Interface
{
    public interface IUserService
    {

        Task<MessageResponse> CreateUser(UserCreateDto request);
        Task<MessageResponse> Login(UserLoginDto request);
        Task<UserCartResponseDto> GetMyCart(int userId);

        UserInfoDto GetUserInfo();
    }
}
