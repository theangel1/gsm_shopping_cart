using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GSM.Data;
using Microsoft.AspNetCore.Mvc;

namespace GSM.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EstadisticasController : Controller
    {

        private readonly ApplicationDbContext _db;

        public EstadisticasController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}