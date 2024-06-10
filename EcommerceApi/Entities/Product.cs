using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceApi.Entities
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int StoreId { get; set; }
        [JsonIgnore]
        public ProductCategory Category { get; set; }
        [JsonIgnore]
        public Store Store { get; set; }

        [JsonIgnore]
        public List<Stock> Stokcs { get; set; }
    }
}
