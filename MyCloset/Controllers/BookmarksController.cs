
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCloset.Data;
using MyCloset.Models;

namespace MyCloset.Controllers
{
    [Authorize]
    public class BookmarksController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public BookmarksController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        // Toti utilizatorii pot vedea Bookmark-urile existente in platforma
        // Fiecare utilizator vede bookmark-urile pe care le-a creat
        // Userii cu rolul de Admin pot sa vizualizeze toate bookmark-urile existente
        // HttpGet - implicit
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();

            if (User.IsInRole("User"))
            {
                var bookmarks = from bookmark in db.Bookmarks.Include("User")
                               .Where(b => b.UserId == _userManager.GetUserId(User))
                                select bookmark;

                ViewBag.Bookmarks = bookmarks;

                return View();
            }
            else
            if (User.IsInRole("Admin"))
            {
                var bookmarks = from bookmark in db.Bookmarks.Include("User")
                                select bookmark;

                ViewBag.Bookmarks = bookmarks;

                return View();
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi asupra colectiei";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Articles");
            }

        }

        // Afisarea tuturor itemurilor pe care utilizatorul le-a salvat in 
        // bookmark-ul sau 
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            SetAccessRights();

            if (User.IsInRole("User"))
            {
                var bookmarks = db.Bookmarks
                                  .Include("ItemBookmarks.Item.Category")
                                  .Include("ItemBookmarks.Item.User")
                                  .Include("User")
                                  .Where(b => b.Id == id)                                 
                                  .FirstOrDefault();

                if (bookmarks == null)
                {
                    TempData["message"] = "Resursa cautata nu poate fi gasita";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Items");
                }

                return View(bookmarks);
            }

            else
            if (User.IsInRole("Admin"))
            {
                var bookmarks = db.Bookmarks
                                  .Include("ItemBookmarks.Item.Category")
                                  .Include("ItemBookmarks.Item.User")
                                  .Include("User")
                                  .Where(b => b.Id == id)
                                  .FirstOrDefault();


                if (bookmarks == null)
                {
                    TempData["message"] = "Resursa cautata nu poate fi gasita";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Items");
                }


                return View(bookmarks);
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Items");
            }
        }

        // Randarea formularului in care se completeaza datele unui bookmark
        // [HttpGet] - se executa implicit
        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            return View();
        }

        // Adaugarea bookmark-ului in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Bookmark bm)
        {
            bm.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Bookmarks.Add(bm);
                db.SaveChanges();
                TempData["message"] = "Colectia a fost adaugata";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                return View(bm);
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var bookmark = await db.Bookmarks.FindAsync(id);
            if (bookmark == null || bookmark.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            return View(bookmark);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Bookmark bookmark)
        {
            if (id != bookmark.Id)
            {
                return NotFound();
            }

            Bookmark bkm = db.Bookmarks.Find(id);

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (bookmark.UserId != user.Id)
                    {
                        return Unauthorized();
                    }

                    bkm.Name = bookmark.Name;
                    bkm.IsPublic = bookmark.IsPublic;
                    
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookmarkExists(bookmark.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(bookmark);

        }
            private bool BookmarkExists(int id)
            {
                return db.Bookmarks.Any(e => e.Id == id);
            }
        


    // Conditiile de afisare a butoanelor de editare si stergere
    private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}
