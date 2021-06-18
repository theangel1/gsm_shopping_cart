using GSM.Data;
using GSM.Extensions;
using GSM.Models;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GSM.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class ContractController : Controller
    {
        public const string LogoEmpresa = @"wwwroot\images\logo.png";
        private readonly ApplicationDbContext _db;


        public ContractController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            //si soy admin, mostrare todos los contractos, si no es asi(usuario normal) muestro solo los del
            //actual user(cliente)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Admin"))
            {
                var contratosAdmin = _db.Contract;
                return View(await contratosAdmin.ToListAsync());
            }

            var contratosFromDB = _db.Contract.Where(c => c.Email == User.Identity.Name);
            return View(await contratosFromDB.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var contratosFromDB = await _db.Contract.FirstOrDefaultAsync(o => o.Id == id);

            if (contratosFromDB == null)
                return NotFound();

            return View(contratosFromDB);
        }

        public IActionResult VerContrato(int? id)
        {
            if (id != null)
            {
                var contratoFromDB = _db.Contract.Where(c => c.Id == id).FirstOrDefault();

                
                if (contratoFromDB.NombreArchivo != null)
                {
                    var directorioFullContrato = @"wwwroot\resources\contratos\" + contratoFromDB.NombreArchivo;
                    var stream = new FileStream(directorioFullContrato, FileMode.Open);
                    return File(stream, "application/pdf");
                }
                else
                    return NotFound();
            }
            else
                return NotFound();
        }



        public IActionResult CreateContractPDF()
        {
            int orderId = HttpContext.Session.Get<int>("ordenCompraId");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFromDB = _db.ApplicationUser.Where(u => u.Id == userId).FirstOrDefault();

            var order = _db.Order.Where(o => o.Id == orderId).FirstOrDefault();

            var nombreArchivo = orderId + ".pdf";

            string directorioPDF = @"wwwroot\resources\contratos\" + nombreArchivo;

            var writer = new PdfWriter(directorioPDF);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            var fuenteParrafo = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.COURIER);
            //var fuenteMarcadaRojo = setFontColor
            var font = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.COURIER_BOLD);
            Image logo = new Image(ImageDataFactory.Create(LogoEmpresa));
            logo.SetWidth(160);
            logo.SetOpacity(80);
            //logo.set;
            Paragraph p = new Paragraph("").Add(logo);

            Paragraph c = new Paragraph("CONTRATO DE COMPRA Y VENTA" +
                "\nFECHA DE ENTREGA " + DateTime.Now.AddDays(30).ToShortDateString() +
                "\nFOLIO " + orderId +
                "\nSANTA MARÍA S.P.A." +
                "\nY" +
                "\n" + userFromDB.RazonSocial
                ).SetFont(font).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

            Paragraph parrafoContrato = new Paragraph("En San Bernardo, con fecha de hoy " + DateTime.Now + ", entre Santa María" +
                "  S.P.A. Giro Compraventa, comercialización de casas prefabricadas, RUT. N°76.548.084-1, Representada" +
                " por Doña MARIA ALEJANDRA BERRIOS MORALES, RUT. 14.437.661-7 Factor de Comercio, ambos domiciliados" +
                " en Avenida Irarrázaval 2821, Comuna de Ñuñoa, Ciudad de SANTIAGO en adelante 'El vendedor' por " +
                "una parte; y por la otra, Don(ña) " + userFromDB.RazonSocial + " con domicilio legal en" +
                " " + userFromDB.Direccion + ", y referencia de entrega en " + userFromDB.Direccion + " " +
                "RUT: " + userFromDB.Rut + ", Nacionalidad CHILENA, teléfonos " + userFromDB.PhoneNumber + " Correo Electrónico" +
                " " + userFromDB.Email + ", en adelante 'El comprador', han convenido en celebrar el siguiente " +
                "Contrato de Compraventa:" +
                "\nPRIMERO: Casas Santa María, es dueña de 1 cabaña(s) en Paneles Prefabricados de, " +
                "YYYYYYYYYYYY ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ. Que consta de las siguientes características" +
                " que el comprador declara conocer y aceptar, libre y espontáneamente, con las siguientes " +
                "características: xxxxx muchas caracteristicas" +
                "\n3 COPIAS DE CERTIFICADO DE CONFORMIDAD(Este Certificado debe ser firmado únicamente por el" +
                " comprador; en caso que no pudiera estar presente al momento del retiro de las especies, quien" +
                " firme en su representación, chofer de camión, familiar u otro, serán los responsables del " +
                "Visto Bueno y conformidad de lo entregado por Santa María S.P.A. No se aceptarán reclamos" +
                " posteriores relacionados a la falta de material o mal estado de estos luego de su retiro" +
                " y posterior firma.)" +
                "\nSEGUNDO: Por el presente instrumento Santa María S.P.A. Vende, cede y transfiere la propiedad" +
                " de los bienes muebles(PANELES forrados por una cara, cerchas, techumbre) señalados en la" +
                " cláusula segunda precedente al comprador quien los compra y adquiere para sí." +
                "\nTERCERO: El precio de la compraventa de la casa es la suma de $$$$TOTAL Que el comprador" +
                " paga en la siguiente forma:" +
                "\na) Con la suma que el Comprador hace pago «TIPO_PAGO» por un monto de $" + order.Total + " . " +
                "Que el vendedor recibe a su entera satisfacción.\nb) El saldo correspondiente a $0(CERO PESOS), " +
                "Que el comprador pagará en efectivo, el día __/ __ / ____ o transferencia antes de la entrega " +
                "de los bienes señalados en la planta de «DIR_SUCU» y antes de ser despachada, " +
                "según cláusula segunda del presente instrumento.\nc) Nota: Es responsabilidad del comprador " +
                "avisar el pago del saldo al vendedor.\nd) «PAGO»" +
                "\nCUARTO: Santa Maria S.P.A. se obliga a entregar los PANELES correspondientes para " +
                "casa Prefabricada el día SSSSSSSSSSSSSSSSSSSSS, a contar de la fecha de suscripción del " +
                "presente instrumento, pudiendo prorrogar dicho plazo, unilateralmente y sin que ello " +
                "constituya simple retardo, mora o incumplimiento contractual ni habilite al comprador " +
                "para demandar indemnización, de ninguna especie o naturaleza, por una sola vez y por un " +
                "periodo no superior a 30 (TREINTA) días hábiles vencido el plazo primitivamente convenido " +
                "y según la disponibilidad en calendario de entrega. Vencido este plazo, SANTA MARIA S.P.A. " +
                "estará facultada para cobrar bodegaje diario equivalente a (1/2 UF)." +
                "\nQUINTO: El vendedor pondrá, en la fecha señalada en la cláusula cuarta o su respectiva" +
                " prórroga, si fuere procedente, a disposición del comprador, satisfaciendo con ello las " +
                "obligaciones que el presente contrato impone a Casas Santa María. La empresa no se hace " +
                "responsable de los deterioros o daños que pudiera afectar a los PANELES Y MATERIALES una " +
                "vez puesta ésta a disposición del comprador y este haya firmado el certificado de conformidad" +
                " respectiva. En relación a la entrega y carga de la casa podra tener una demora de 4 a 5 horas." +
                "\nSEXTO: Se deja constancia que, a fin de garantizar la venta, el comprador no podrá desistir " +
                "de ella, por lo cual se establece una clausula penal a favor de Santa Maria S.P.A. equivalente" +
                " al 25% del valor inicial de venta, la cual será pagada en un periodo de 90 días hábiles luego" +
                " de ser solicitada a través de una carta firmada por el comprador o correo electrónico al " +
                "supervisor de ventas toda devolución será efectiva dentro de 90 días hábiles posteriores al " +
                "pago del bien, posterior a esto el comprador pierde su derecho a cualquier devolución monetaria" +
                " (indicación sujeta  a la Ley del Consumidor). Ademas no podrá solicitar devolucion de dinero " +
                "despues de un año cumplido a la fecha de celebracion del contrato." +
                "\nSEPTIMO: Ante cualquier dificultad que surja respecto de la interpretación o aplicación " +
                "del presente  contrato, las partes prorrogan competencia a los Tribunales Ordinarios de " +
                "la ciudad de San Bernardo.\nOCTAVO: El presente instrumento se suscribe en tres ejemplares " +
                "quedando uno en poder del comprador y uno en poder de Casas Santa Maria." +
                "\nNOVENO: Los kits básicos y completos NO incluyen, limahoyas, junquillos, chapas y clavos de " +
                "ningún tipo. Además no se contempla el armado de los PANELES en terreno, este es de " +
                "responsabilidad del comprador.\nNOTA: Las cantidades de tablas en medialuna van ubicadas para " +
                "cada metraje según los PANELES, por lo tanto no se aceptará reclamo alguno, si el maestro no " +
                "supiera instalar estas, ya que es responsabilidad del comprador tener un profesional capacitado " +
                "en el armado.").SetFont(fuenteParrafo);

            document.Add(p);
            document.Add(c);
            document.Add(parrafoContrato);

            document.Close();

            var stream = new FileStream(directorioPDF, FileMode.Open);

            var contract = new Contract
            {
                OrderId = orderId,
                Email = userFromDB.Email,
                Descripcion = "Descripcion",
                Fecha = DateTime.Now,
                FechaEntrega = DateTime.Now.AddDays(30),
                RazonSocial = userFromDB.RazonSocial,
                Telefono = userFromDB.PhoneNumber,
                Rut = userFromDB.Rut,
                Total = order.Total,
                NombreArchivo = nombreArchivo
            };
            _db.Add(contract);
            _db.SaveChanges();

            return File(stream, "application/pdf");



        }
    }
}