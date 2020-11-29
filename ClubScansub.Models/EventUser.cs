using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubScansub.Models
{
    [Table("EventUser")]
    public class EventUser
    {
        [Key]
        public string ApplicationUserId { get; set; }
        [Key]
        public int EventId { get; set; }
        [JsonIgnore]
        public ApplicationUser ApplicationUser { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }
    }
}
