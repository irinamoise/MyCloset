using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCloset.Models;

namespace MyCloset.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<ItemBookmark> ItemBookmarks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // definirea relatiei many-to-many dintre Item si Bookmark

            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<ItemBookmark>()
                .HasKey(ab => new { ab.Id, ab.ItemId, ab.BookmarkId });


            // definire relatii cu modelele Bookmark si Item (FK)

            modelBuilder.Entity<ItemBookmark>()
                .HasOne(ab => ab.Item)
                .WithMany(ab => ab.ItemBookmarks)
                .HasForeignKey(ab => ab.ItemId);

            modelBuilder.Entity<ItemBookmark>()
                .HasOne(ab => ab.Bookmark)
                .WithMany(ab => ab.ItemBookmarks)
                .HasForeignKey(ab => ab.BookmarkId);
        }


    }


}
