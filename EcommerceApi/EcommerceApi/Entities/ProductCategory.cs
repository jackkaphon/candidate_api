using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceApi.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; }
    }
}
