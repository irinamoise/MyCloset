
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyCloset.Data;
using MyCloset.Models;
using System.Linq;

namespace MyCloset.Controllers
{
    
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly IWebHostEnvironment _env;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
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

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            ViewBag.UserCurent = await _userManager.GetUserAsync(User);

            ViewBag.Items = db.Items.Where(item => item.UserId == id).ToList();

            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.UserCurent = user;
            ViewBag.Roles = roles;
            ViewBag.Items = db.Items.Where(item => item.UserId == user.Id).ToList();

            return View("Show", user);
        }

        [Authorize]
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            ViewBag.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user); // Lista de nume de roluri

            // Cautam ID-ul rolului in baza de date
            ViewBag.UserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First(); // Selectam 1 singur rol

            return View(user);
        }

        
        //[HttpPost]
        //public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        //{
        //    ApplicationUser user = db.Users.Find(id);

        //    user.AllRoles = GetAllRoles();


        //    if (ModelState.IsValid)
        //    {
        //        user.UserName = newData.UserName;
        //        user.Email = newData.Email;
        //        user.FirstName = newData.FirstName;
        //        user.LastName = newData.LastName;
        //        user.PhoneNumber = newData.PhoneNumber;
        //        user.Bio = newData.Bio;

        //        // Cautam toate rolurile din baza de date
        //        var roles = db.Roles.ToList();

        //        foreach (var role in roles)
        //        {
        //            // Scoatem userul din rolurile anterioare
        //            await _userManager.RemoveFromRoleAsync(user, role.Name);
        //        }
        //        // Adaugam noul rol selectat
        //        var roleName = await _roleManager.FindByIdAsync(newRole);
        //        await _userManager.AddToRoleAsync(user, roleName.ToString());

        //        db.SaveChanges();

        //    }
        //    return RedirectToAction("Index");
        //}

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole, [FromForm] IFormFile ProfilePicture)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();

            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;
                user.FirstName = newData.FirstName;
                user.LastName = newData.LastName;
                user.PhoneNumber = newData.PhoneNumber;
                user.Bio = newData.Bio;

                if (ProfilePicture != null && ProfilePicture.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov" };
                    var fileExtension = Path.GetExtension(ProfilePicture.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ArticleImage", "Fișierul trebuie să fie o imagine(jpg, jpeg, png, gif) sau un video(mp4, mov).");
                        return View(user);
                    }

                    // Cale stocare
                    var storagePath = Path.Combine(_env.WebRootPath, "images", ProfilePicture.FileName);

                    var databaseFileName = "/images/" + ProfilePicture.FileName;
                    // Salvare fișier
                    using (var fileStream = new FileStream(storagePath, FileMode.Create))
                    {
                        await ProfilePicture.CopyToAsync(fileStream);
                    }

                    ModelState.Remove(nameof(user.ProfilePicture));
                    user.ProfilePicture = databaseFileName;

                }

                //modificare poza de profil in baza de date
                if(TryValidateModel(user))
                {
                    db.Entry(user).State = EntityState.Modified;
                }

                // Cautam toate rolurile din baza de date
                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    // Scoatem userul din rolurile anterioare
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                // Adaugam noul rol selectat
                var roleName = await _roleManager.FindByIdAsync(newRole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                db.SaveChanges();
            }
            db.SaveChanges();
            return RedirectToAction("MyProfile");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            var user = db.Users
                         .Include("Items")
                         .Include("Comments")
                         .Include("Bookmarks")
                         .Where(u => u.Id == id)
                         .First();

            // Delete user comments
            if (user.Comments.Count > 0)
            {
                foreach (var comment in user.Comments)
                {
                    db.Comments.Remove(comment);
                }
            }

            // Delete user bookmarks
            if (user.Bookmarks.Count > 0)
            {
                foreach (var bookmark in user.Bookmarks)
                {
                    db.Bookmarks.Remove(bookmark);
                }
            }

            // Delete user item
            if (user.Items.Count > 0)
            {
                foreach (var article in user.Items)
                {
                    db.Items.Remove(article);
                }
            }

            db.ApplicationUsers.Remove(user);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        private void SetAccessRights()
        {
            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}
