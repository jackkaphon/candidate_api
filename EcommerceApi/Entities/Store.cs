using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceApi.Entities
{
    public class Store:BaseEntity
    {
        public string Name { get; set; }

        public int OwnerId {  get; set; }   

        public User Owner { get; set; }

        [JsonIgnore]
        public List<User> Managers { get; set; }
        [JsonIgnore]
        public List<Product> Products { get; set; }
    }
}
