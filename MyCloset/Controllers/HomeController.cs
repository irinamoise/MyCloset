using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCloset.Data;
using MyCloset.Models;
using System.Diagnostics;

namespace MyCloset.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext db;
        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            db = context;
        }

        public async Task<IActionResult> Index(string sortOrder, int page = 1)
        {
            ViewBag.SortOrder = sortOrder;

            var items = from item in db.Items
                        select item;

            switch (sortOrder)
            {
                case "most_liked":
                    items = items.OrderByDescending(i => i.Likes).ThenBy(i => i.Name);
                    break;
                case "most_recent":
                default:
                    items = items.OrderByDescending(i => i.Date);
                    break;
            }

            int _perPage = 2;
            int totalItems = await items.CountAsync();
            var offset = (page - 1) * _perPage;

            var paginatedItems = await items.Skip(offset).Take(_perPage).ToListAsync();

            ViewBag.FirstItem = items.FirstOrDefault();
            ViewBag.Items = paginatedItems;
            ViewBag.CurrentPage = page;
            ViewBag.LastPage = (int)Math.Ceiling((double)totalItems / _perPage);
            ViewBag.PaginationBaseUrl = "/Home/Index?sortOrder=" + sortOrder + "&page=";

            return View(paginatedItems);
        }

        //public IActionResult Index()
        //{
        //    //if (User.Identity.IsAuthenticated)
        //    //{
        //    //    return RedirectToAction("Index", "Items");
        //    //}
        //    var items = from item in db.Items
        //                select item;
        //    ViewBag.FirstItem = items.First();
        //    ViewBag.Items = items.OrderBy(o => o.Date).Skip(1).Take(2);
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
