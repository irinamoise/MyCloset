using MyCloset.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCloset.Models
{
    public class ItemBookmark
    {
        // tabelul asociativ care face legatura intre Item si Bookmark
        // un item are mai multe colectii din care face parte
        // iar o colectie contine mai multe items in cadrul ei
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // cheie primara compusa (Id, ArticleId, BookmarkId)
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? BookmarkId { get; set; }
        public virtual Item? Item { get; set; }
        public virtual Bookmark? Bookmark { get; set; }
        public DateTime BookmarkDate { get; set; }
        
    }
}
