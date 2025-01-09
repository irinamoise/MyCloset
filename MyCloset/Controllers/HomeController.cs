using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Index", "Items");
            //}
            var items = from item in db.Items
                        select item;
            ViewBag.FirstItem = items.First();
            ViewBag.Items = items.OrderBy(o => o.Date).Skip(1).Take(2);
            return View();
        }

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
