using EcommerceApi.Entities;
using System.Text.Json.Serialization;

namespace EcommerceApi.Dto.UserDto
{
    public class UserStoreResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }    
        public List<string> Roles { get; set; }       
        public List<Store> StoresOwner { get; set; }
        public List<Store> ManagerOfStores { get; set; }
    }
}
