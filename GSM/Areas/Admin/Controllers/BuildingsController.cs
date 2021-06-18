using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GSM.Data;
using GSM.Models;
using GSM.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GSM.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public class BuildingsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public Building House { get; set; }

        public BuildingsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Building.ToListAsync());
        }

        //get create
        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult Create()
        {
            return View();
        }

        //POST create
        [Authorize(Roles = SD.AdminEndUser)]
        [HttpPost, ValidateAntiForgeryToken, ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
                return View(House);

            _db.Add(House);
            await _db.SaveChangesAsync();

            //Grabar imagen
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var HousesFromDb = _db.Building.Find(House.Id);

            if (files.Count != 0)
            {
                //Image has been uploaded
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);

                var extension = Path.GetExtension(files[0].FileName);
                var extensionPlano = Path.GetExtension(files[1].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, House.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                //Arreglo para subir la imagen del plano de la casa
                using (var filestream = new FileStream(Path.Combine(uploads, House.Id +"_plano"+ extensionPlano), FileMode.Create))
                {
                    files[1].CopyTo(filestream);
                }

                HousesFromDb.ImagenPlano = @"\" + SD.ImageFolder + @"\" + House.Id +"_plano" + extensionPlano;
                HousesFromDb.Imagen = @"\" + SD.ImageFolder + @"\" + House.Id + extension;
            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + House.Id + ".png");
                HousesFromDb.Imagen = @"\" + SD.ImageFolder + @"\" + House.Id + ".png";
                HousesFromDb.ImagenPlano = @"\" + SD.ImageFolder + @"\" + House.Id+"_plano" + ".png";
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //get edit
        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var casa = await _db.Building.FindAsync(id);

            if (casa == null)
                return NotFound();

            return View(casa);
        }

        //post edit
        [Authorize(Roles = SD.AdminEndUser)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (id != House.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var buildingFromDb = _db.Building.Where(m => m.Id == House.Id).FirstOrDefault();

                if (files.Count > 0 && files[0] != null && files[1] != null)
                {
                    //if user uploads a new image
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);

                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(buildingFromDb.Imagen);

                    var extension_new_plano = Path.GetExtension(files[1].FileName);
                    var extension_old_plano = Path.GetExtension(buildingFromDb.ImagenPlano);

                    if (System.IO.File.Exists(Path.Combine(uploads, House.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, House.Id + extension_old));
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, House.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    //agrego img plano
                    if (System.IO.File.Exists(Path.Combine(uploads, House.Id + extension_old_plano)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, House.Id + extension_old_plano));
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, House.Id + "_plano"+extension_new_plano), FileMode.Create))
                    {
                        files[1].CopyTo(filestream);
                    }

                    House.Imagen = @"\" + SD.ImageFolder + @"\" + House.Id + extension_new;
                    House.ImagenPlano = @"\" + SD.ImageFolder + @"\" + House.Id +"_plano" +extension_new_plano;
                }


                if (House.Imagen != null)                
                    buildingFromDb.Imagen = House.Imagen;

                if (House.ImagenPlano != null)
                    buildingFromDb.ImagenPlano = House.ImagenPlano;
               

                buildingFromDb.Descripcion = House.Descripcion;
                buildingFromDb.HasCocinaAmericana = House.HasCocinaAmericana;
                buildingFromDb.HasLivingComedor = House.HasLivingComedor;
                buildingFromDb.HasPorche = House.HasPorche;
                buildingFromDb.NumeroBano = House.NumeroBano;
                buildingFromDb.NumeroDormitorios = House.NumeroDormitorios;
                buildingFromDb.Precio = House.Precio;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(House);

        }


        /* La lógica de este metodo, es que hare una direccion al details que se encuentra en el area
         de Customer, en el controlador de home. Esto para utilizar el carro de compras ya que toda la logica
         está aplicada ahí.*/
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction("Details", "Home", new { id, Area="Customer"}  );
           
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)           
                return NotFound();            

            var casa = await _db.Building.FirstOrDefaultAsync(m => m.Id == id);

            if (casa == null)            
                return NotFound();            

            return View(casa);
        }
        [Authorize(Roles = SD.AdminEndUser)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var casa = await _db.Building.FindAsync(id);
            _db.Building.Remove(casa);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool BuildingExists(int id)
        {
            return _db.Building.Any(e => e.Id == id);
        }

    }
}