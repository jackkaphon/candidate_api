using EcommerceApi.Dto.UserDto;

namespace EcommerceApi.Dto.StoreDto
{
    public class StoreResponseDto
    {
        public int  Id { get; set; }
        public  string Name { get; set; }

        public UserResponseDto Owner { get; set; }
        public List<UserResponseDto> Managers { get; set; }
    }
}
