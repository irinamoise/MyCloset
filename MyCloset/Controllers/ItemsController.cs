﻿using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyCloset.Data;
using MyCloset.Models;
using static MyCloset.Models.ItemBookmark;

namespace MyCloset.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly IWebHostEnvironment _env;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public ItemsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env
        )
        {
            db = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // Se afiseaza lista tuturor articolelor impreuna cu categoria 
        // din care fac parte
        // HttpGet implicit
        public IActionResult Index()
        {
            var items = db.Items.Include("Category");

            // ViewBag.OriceDenumireSugestiva
            ViewBag.Items = items;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // Se afiseaza un singur articol in functie de id-ul sau 
        // impreuna cu categoria din care face parte
        // In plus sunt preluate si toate comentariile asociate unui articol
        // Se afiseaza si userul care a postat articolul respectiv
        // [HttpGet] se executa implicit implicit
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            Item item = db.Items.Include("Category")
                                    .Include("Comments")
                                    .Include("User")
                                    .Include("Comments.User")
                                    .Where(it => it.Id == id)
                                    .First();

            // Adaugam bookmark-urile utilizatorului pentru dropdown
            ViewBag.UserBookmarks = db.Bookmarks
                                      .Where(b => b.UserId == _userManager.GetUserId(User))
                                      .ToList();

            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(item);
        }

        // Se afiseaza formularul in care se vor completa datele unui articol
        // impreuna cu selectarea categoriei din care face parte
        // HttpGet implicit


        // Adaugarea unui comentariu asociat unui articol in baza de date
        
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;

            // preluam Id-ul utilizatorului care posteaza comentariul
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Items/Show/" + comment.ItemId);
            }
            else
            {
                Item it = db.Items.Include("Category")
                                  .Include("User")
                                  .Include("Comments")
                                  .Include("Comments.User")
                                  .Where(art => art.Id == comment.ItemId)
                                  .First();

                //return Redirect("/Articles/Show/" + comm.ArticleId);

                // Adaugam bookmark-urile utilizatorului pentru dropdown
                ViewBag.UserBookmarks = db.Bookmarks
                                          .Where(b => b.UserId == _userManager.GetUserId(User))
                                          .ToList();

                SetAccessRights();

                return View(it);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult AddBookmark([FromForm] ItemBookmark itemBookmark)
        {
            // Daca modelul este valid
            if (ModelState.IsValid)
            {
                // Verificam daca avem deja itemul in colectie
                if (db.ItemBookmarks
                    .Where(ab => ab.ItemId == itemBookmark.ItemId)
                    .Where(ab => ab.BookmarkId == itemBookmark.BookmarkId)
                    .Count() > 0)
                {
                    TempData["message"] = "Acest item este deja adaugat in colectie";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    // Adaugam asocierea intre articol si bookmark 
                    db.ItemBookmarks.Add(itemBookmark);
                    // Salvam modificarile
                    db.SaveChanges();

                    // Adaugam un mesaj de succes
                    TempData["message"] = "Itemul a fost adaugat in colectia selectata";
                    TempData["messageType"] = "alert-success";
                }

            }
            else
            {
                TempData["message"] = "Nu s-a putut adauga itemul in colectie";
                TempData["messageType"] = "alert-danger";
            }

            // Ne intoarcem la pagina itemului
            return Redirect("/Items/Show/" + itemBookmark.ItemId);
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Item item= new Item();

            item.Categ = GetAllCategories();

            return View(item);
        }

        // Se adauga itemul in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> New(Item item, IFormFile Image)
        {

            item.Date = DateTime.Now;
            item.Categ = GetAllCategories();

            item.UserId = _userManager.GetUserId(User);

            if (Image != null && Image.Length > 0)
            {
                // Verificăm extensia
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif",".mp4", ".mov" };
                var fileExtension = Path.GetExtension(Image.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ArticleImage", "Fișierul trebuie să fie o imagine(jpg, jpeg, png, gif) sau un video(mp4, mov).");
                return View(item);
                }

                // Cale stocare
                var storagePath = Path.Combine(_env.WebRootPath, "images", Image.FileName);

                var databaseFileName = "/images/" + Image.FileName;
                // Salvare fișier
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

                ModelState.Remove(nameof(item.Image));
                item.Image = databaseFileName;
            }

            if (TryValidateModel(item))
            {
                // Adăugare articol
                db.Items.Add(item);
                await db.SaveChangesAsync();
                TempData["message"] = "Itemul a fost adaugat";
                TempData["messageType"] = "alert-success";
                // Redirecționare după succes
                return RedirectToAction("Index", "Items");
            }
            item.Categ = GetAllCategories();
            return View(item);
        }

        // Se editeaza un articol existent in baza de date impreuna cu categoria din care face parte
        // Categoria se selecteaza dintr-un dropdown
        // HttpGet implicit
        // Se afiseaza formularul impreuna cu datele aferente articolului din baza de date

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Item item = db.Items.Include("Category")
                                         .Where(art => art.Id == id)
                                         .First();

            item.Categ = GetAllCategories();


            if (item.UserId == _userManager.GetUserId(User) ||User.IsInRole("Admin"))
            {
                return View(item);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui item care nu va apartine";
            return RedirectToAction("Index");
            }

        }

        // Se adauga articolul modificat in baza de date
        [HttpPost]

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Item requestItem)
        {
            Item item = db.Items.Find(id);

            if (ModelState.IsValid)
            {
                if (item.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    item.Name = requestItem.Name;
                    item.Caption = requestItem.Caption;
                    item.Date = DateTime.Now;
                    item.CategoryId = requestItem.CategoryId;
                    TempData["message"] = "Itemul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui item care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                requestItem.Categ = GetAllCategories();
                return View(requestItem);
            }

        }


        // Se sterge un item din baza de date 
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Item item = db.Items.Include("Comments")
                                         .Where(art => art.Id == id)
                                         .First();

            if (item.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Items.Remove(item);
                db.SaveChanges();
                TempData["message"] = "Itemul a fost sters";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui item care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            
            
        }

        // Conditiile de afisare pentru butoanele de editare si stergere
        // butoanele aflate in view-uri
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            ViewBag.UserCurent = _userManager.GetUserId(User);

            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName
                });
            }
            


            // returnam lista de categorii
            return selectList;
        }

        
    }

    

   
}

