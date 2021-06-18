using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GSM.Data;
using GSM.Models;
using GSM.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GSM.Utility;

namespace GSM.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public OrdenDetalleViewModel OrdenVM { get; set; }

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
            OrdenVM = new OrdenDetalleViewModel()
            {
                Order = new Order(),
                OrderDetail = new List<OrderDetail>()
            };
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            //En esta consulta solo muestro las ordenes del usuario.Aunque si es admin deberia mostrar todo... no?
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
          
            if (User.IsInRole("Admin"))
            {
                var query = _db.Order.Include(o => o.ApplicationUser);
                return View(await query.ToListAsync());

            }
            var applicationDbContext = _db.Order.Include(o => o.ApplicationUser).Where(o => o.ApplicationUser.Id == userId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            OrdenVM.Order = await _db.Order.FirstOrDefaultAsync(o => o.Id == id);

            if (OrdenVM.Order == null)
                return NotFound();

            List<OrderDetail> objDetalle = _db.OrderDetail.Include(d => d.Building).Where(d => d.OrderId == id).ToList();

            foreach (OrderDetail detail in objDetalle)
            {
                OrdenVM.OrderDetail.Add(detail);
            }
            return View(OrdenVM);
        }

        // GET: Admin/Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_db.ApplicationUser, "Id", "Id");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderDate,UserId,FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email,Total,PaymentTransactionId,HasBeenShipped")] Order order)
        {
            if (ModelState.IsValid)
            {
                _db.Add(order);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_db.ApplicationUser, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        [Authorize(Roles =SD.AdminEndUser)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _db.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_db.ApplicationUser, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,UserId,FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email,Total,PaymentTransactionId,HasBeenShipped")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(order);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_db.ApplicationUser, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _db.Order
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _db.Order.FindAsync(id);
            _db.Order.Remove(order);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _db.Order.Any(e => e.Id == id);
        }
    }
}
