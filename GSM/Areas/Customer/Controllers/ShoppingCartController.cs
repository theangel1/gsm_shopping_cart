using GSM.Data;
using GSM.Extensions;
using GSM.Models;
using GSM.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GSM.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;        

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
       

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;            

            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Buildings = new List<Building>()                
            };
        }

        //Get Index Shopping Cart
        public async Task<IActionResult> Index()
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if ((lstShoppingCart != null) && (lstShoppingCart.Any()))
            {
                foreach (int cartItem in lstShoppingCart)
                {
                    Building build = await _db.Building.Where(p => p.Id == cartItem).FirstOrDefaultAsync();
                    ShoppingCartVM.Buildings.Add(build);
                }
            }

            
            return View(ShoppingCartVM);
        }

        [Authorize]
        [HttpPost,ValidateAntiForgeryToken, ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            List<Building> misCasas = new List<Building>();
            double PrecioFinal = 0;

            //Logica de post para almacenar orden y pagar por webpay y paypal??????
            foreach (int idBuilding in lstCartItems)
            {
                var casaFromDB = _db.Building.Where(b => b.Id == idBuilding).FirstOrDefault();
                misCasas.Add(casaFromDB);
                PrecioFinal += casaFromDB.Precio;
            }

            //ShoppingCartVM.Order.OrderDate = DateTime.Today;
            //ShoppingCartVM.Order.Total = PrecioFinal;

            Order ordenes = new Order();
            ordenes.OrderDate = DateTime.Today;
            ordenes.Total = PrecioFinal;
            
            _db.Order.Add(ordenes);
            _db.SaveChanges();

            int orderCompraId = ordenes.Id;

            foreach (var objOrderDetail in misCasas)
            {
                OrderDetail detalles = new OrderDetail()
                {
                    OrderId = orderCompraId,
                    BuildingId = objOrderDetail.Id,
                    Quantity = 1,
                    UnitPrice = objOrderDetail.Precio
                    
                };
                _db.OrderDetail.Add(detalles);
            }
            _db.SaveChanges();
            lstCartItems = new List<int>();
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            return RedirectToAction("OrderConfirmation", "ShoppingCart", new { Id = orderCompraId });
        }

        public IActionResult Remove(int id)
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstCartItems.Count > 0)
            {
                if (lstCartItems.Contains(id))
                {
                    lstCartItems.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            return RedirectToAction(nameof(Index));
        }

        //GET
        [Authorize]
        public IActionResult OrderConfirmation(int id)
        {
            //Obtener el user id para ponerlo en la tabla correspondiente
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromDB = _db.ApplicationUser.Where(u => u.Id == userId).FirstOrDefault();            

            ShoppingCartVM.Order = _db.Order.Where(a => a.Id == id).FirstOrDefault();
            ShoppingCartVM.Order.FirstName = userFromDB.RazonSocial;
            ShoppingCartVM.Order.Phone = userFromDB.PhoneNumber;
            List<OrderDetail> objCasasList = _db.OrderDetail.Include(o => o.Building).Where(o => o.OrderId == id).ToList();

            HttpContext.Session.Set("ordenCompraId", id);
            HttpContext.Session.Set("objCasasList", objCasasList);
            HttpContext.Session.Set("payment_amt", ShoppingCartVM.Order.Total);

            foreach (OrderDetail casasDetalleObj in objCasasList)
            {
                ShoppingCartVM.Buildings.Add(_db.Building.Where(p => p.Id == casasDetalleObj.BuildingId).FirstOrDefault());
            }
            return View(ShoppingCartVM);
        }
    }
}