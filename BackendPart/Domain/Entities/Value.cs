using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Value
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PropertyId { get; set; }
        [JsonIgnore]
        public Property Property { get; set; }
        [JsonIgnore]
        public IEnumerable<Mark> Marks { get; set; } = new List<Mark>();
    }
}
