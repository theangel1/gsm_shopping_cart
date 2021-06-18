using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GSM.Models;
using GSM.Data;
using Microsoft.EntityFrameworkCore;
using GSM.Extensions;

namespace GSM.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var buildingList = await _db.Building.ToListAsync();

           /* var tempMessage = TempData["ProductoAgregado"];
            var tempMessEliminado = TempData["ProductoEliminado"];

            if (tempMessage != null)
                ViewData["ProductoAgregado"] = tempMessage;

            if (tempMessEliminado != null)
                ViewData["ProductoEliminado"] = tempMessEliminado;*/

            return View(buildingList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var house = await _db.Building.Where(m => m.Id == id).FirstOrDefaultAsync();

            return View(house);
        }

        [HttpPost, ActionName("Details"), ValidateAntiForgeryToken]
        public IActionResult DetailsPost(int id)
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart == null)
            {
                lstShoppingCart = new List<int>();
            }
            lstShoppingCart.Add(id);

            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);
            //TempData["ProductoAgregado"] = "Item Agregado";

            return RedirectToAction("Index", "ShoppingCart", new { area = "Customer" });

        }

        public IActionResult Remove(int id)
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart.Count > 0)
            {
                if (lstShoppingCart.Contains(id))
                {
                    lstShoppingCart.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);
            TempData["ProductoEliminado"] = "Item Eliminado";

            return RedirectToAction(nameof(Index));
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
