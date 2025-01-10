
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace MyCloset.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Denumirea este obligatorie")]
        [StringLength(100, ErrorMessage = "Denumirea nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Denumirea trebuie sa aiba mai mult de 5 caractere")]
        public string Name { get; set; }

        //[Max200CharsValidation] - Validare custom folosind atribute personalizate
        [Required(ErrorMessage = "Descrierea item-ului este obligatorie")]

        public string Caption { get; set; }

        public DateTime Date { get; set; }


        [Required(ErrorMessage = "Incarcarea unei imagini este obligatorie!")]
        public string Image { get; set; }

        // string pt salvarea path-ului imaginii pt item

        [Required(ErrorMessage = "Categoria este obligatorie")]
        // cheie externa (FK) - un articol are asociata o categorie
        public int? CategoryId { get; set; }


        // cheie externa (FK) - un articol este postat de catre un user
        public string? UserId { get; set; }

        // un item are o categorie
        public virtual Category? Category { get; set; }

        // proprietatea virtuala - un articol este postat de catre un user
        public virtual ApplicationUser? User { get; set; }

        // un item poate avea o colectie de comentarii
        public virtual ICollection<Comment>? Comments { get; set; }

        // un item poate avea o colectie de taguri
        //public virtual ICollection<Tag >? Tags { get; set; }

        // relatia many-to-many dintre Item si Bookmark
        public virtual ICollection<ItemBookmark>? ItemBookmarks { get; set; }

        // rel many-to-many dintre Item si Tag
        //public virtual ICollection<ItemTag>? ItemTags { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }

        // [NotMapped]
        //   public IEnumerable<SelectListItem>? AllTags { get; set; }

        public int Likes { get; set; }
        public virtual ICollection<ItemLike>? ItemLikes { get; set; }
    }

}


