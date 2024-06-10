using EcommerceApi.Entities;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace EcommerceApi.Dto.UserDto
{
    public class UserInfoDto
    {
        public string UserId { get; set; }
        public List<Claim> OwnerOfStores { get; set; }                                                               
        public List<Claim> ManagerOfStores { get; set; }
    }
}
