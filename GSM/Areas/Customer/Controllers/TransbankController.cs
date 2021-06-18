using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GSM.Models;
using GSM.Utility;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Webpay.Transbank.Library;
using Webpay.Transbank.Library.Wsdl.Normal;
using System.Web.Services;

namespace GSM.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TransbankController : Controller
    {
        
        public string angel()
        {
            string message = string.Empty;
            string sample_baseurl = Request.GetDisplayUrl();
            string tx_step = "";
            Dictionary<string, string> certificado = CertNormal.Certificate();
            Dictionary<string, string> request = new Dictionary<string, string>();
            
            Configuration configuration = new Configuration
            {
                Environment = certificado["environment"],
                CommerceCode = certificado["commerce_code"],
                PublicCert = certificado["public_cert"],
                WebpayCert = certificado["webpay_cert"],
                Password = certificado["password"]
            };



             

            
             var webpay = new Webpay.Transbank.Library.Webpay(configuration);

             Dictionary<string, string> description = new Dictionary<string, string>
             {
                 { "VD", "Venta Deb&iacute;to" },
                 { "VN", "Venta Normal" },
                 { "VC", "Venta en cuotas" },
                 { "SI", "cuotas sin inter&eacute;s" },
                 { "S2", "2 cuotas sin inter&eacute;s" },
                 { "NC", "N cuotas sin inter&eacute;s" }
             };

             Dictionary<string, string> codes = new Dictionary<string, string>
             {
                 { "0", "Transacci&oacute;n aprobada" },
                 { "-1", "Rechazo de transacci&oacute;n" },
                 { "-2", "Transacci&oacute;n debe reintentarse" },
                 { "-3", "Error en transacci&oacute;n" },
                 { "-4", "Rechazo de transacci&oacute;n" },
                 { "-5", "Rechazo por error de tasa" },
                 { "-6", "Excede cupo m&aacute;ximo mensual" },
                 { "-7", "Excede l&iacute;mite diario por transacci&oacute;n" },
                 { "-8", "Rubro no autorizado" }
             };

             tx_step = "Init";
             try
             {
                 var amount = 1000;
                 var sessionId = "mi-id-de-sesion-angelito";
                 var buyOrder = new Random().Next(100000, 999999999).ToString();
                 var returnUrl = sample_baseurl + "/result";
                 var finalUrl = sample_baseurl + "/end";

                 request.Add("amount", amount.ToString());
                 request.Add("buyOrder", buyOrder.ToString());
                 request.Add("sessionId", sessionId.ToString());
                 request.Add("urlReturn", returnUrl.ToString());
                 request.Add("urlFinal", finalUrl.ToString());

                 wsInitTransactionOutput result = webpay.getNormalTransaction().initTransaction(amount, buyOrder, sessionId, returnUrl, finalUrl);

                 if (result.token != null && result.token != "")
                 {
                     message = "<br/><div class='alert alert-success' role='alert'>Sesion iniciada con exito en Webpay</div>";
                 }
                 else
                 {
                     message = "<br/><div class='alert alert-success' role='alert'>Webpay no disponible</div>";
                 }

             }
             catch (Exception ex)
             {
                 message = ex.ToString();
             }
             
            return message;

        }

        public IActionResult Index()
        {
            return RedirectToRoute("https://localhost:54128/sample/default.aspx");            
        }

        public IActionResult result()
        {
            return View();
        }

        public IActionResult end()
        {
            return View();
        }

    }
}