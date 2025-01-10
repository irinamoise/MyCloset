using static MyCloset.Models.ItemBookmark;
using System.ComponentModel.DataAnnotations;

namespace MyCloset.Models
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele colectiei este obligatoriu")]
        public string Name { get; set; }

        // o colectie este creata de catre un user
        public string? UserId { get; set; }

        public bool IsPublic { get; set; }
        public virtual ApplicationUser? User { get; set; }

        // relatia many-to-many dintre Item si Bookmark
        public virtual ICollection<ItemBookmark>? ItemBookmarks { get; set; }

    }
}
