using System.Collections.Generic;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ShopId { get; set; }
        public Shop Shop { get; set; }
        public IEnumerable<Mark> Marks { get; set; } = new List<Mark>();
    }
}
