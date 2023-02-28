using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Mark
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
        public int ValueId { get; set; }
        public Value Value { get; set; }
    }
}
