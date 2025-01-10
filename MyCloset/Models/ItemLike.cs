using System.ComponentModel.DataAnnotations;

namespace MyCloset.Models
{
    public class ItemLike
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string UserId { get; set; }
        public virtual Item Item { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

}
