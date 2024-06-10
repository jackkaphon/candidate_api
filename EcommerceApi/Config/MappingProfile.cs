using AutoMapper;
using EcommerceApi.Dto.StoreDto;
using EcommerceApi.Dto.UserDto;
using EcommerceApi.Entities;

namespace EcommerceApi.Config
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserCreateDto, User>();
            CreateMap<Store, StoreResponseDto>();
        }
    }
}
