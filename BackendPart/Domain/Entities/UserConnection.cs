using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class UserConnection
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ShopId { get; set; }
        [JsonIgnore]
        public Shop Shop { get; set; }
    }
}
