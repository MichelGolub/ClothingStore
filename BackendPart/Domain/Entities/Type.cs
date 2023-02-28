using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Type
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public IEnumerable<Property> Properties { get; set; } = new List<Property>();

        public enum Types 
        {
            category,
            numeric
        }
    }
}
