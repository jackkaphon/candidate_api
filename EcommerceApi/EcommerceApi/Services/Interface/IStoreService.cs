using EcommerceApi.Dto.StoreDto;
using EcommerceApi.Entities;
using EcommerceApi.Utils;

namespace EcommerceApi.Services.Interface
{
    public interface IStoreService
    {
        Task<List<StoreResponseDto>> GetAllStoresAsync(int ownerId);
        Task<StoreResponseDto> GetStoreByIdAsync(int storeId);
        Task<MessageResponse> AddStoreAsync(StoreCreateDto request);
        Task<Store> UpdateStoreAsync(int storeId, StoreUpdateDto request);
        Task DeleteStoreAsync(int storeId);
        Task<MessageResponse> AddManager(int storeId, int userId);
        Task<MessageResponse> Revork(int storeId, int userId);

    }
}
