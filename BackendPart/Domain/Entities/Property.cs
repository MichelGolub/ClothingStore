using System.Collections.Generic;

namespace Domain.Entities
{
    public class Property
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public Type Type { get; set; }
        public int? ShopId { get; set; }
        public Shop Shop { get; set; }
        public IEnumerable<Value> Values { get; set; } = new List<Value>();
    }
}
