using System.Collections.Generic;

namespace Domain.Entities
{
    public class Shop
    {
        public int Id { get; set; }
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Property> Properties { get; set; } = new List<Property>();
    }
}
