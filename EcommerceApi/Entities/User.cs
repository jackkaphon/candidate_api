using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Data;

namespace EcommerceApi.Entities
{
    public class User:BaseEntity
    {
        public string Username { get; set; } = "";
        public string? Email { get; set; }
        public string PasswordHash { get; set; } = "";
        public List<string>? Roles { get; set; }
        [JsonIgnore]
        public List<Store> Stores { get; set; }
    }
}
