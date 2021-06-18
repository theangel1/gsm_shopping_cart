using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using GSM.Data;
using GSM.Extensions;
using GSM.Models;
using GSM.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace GSM.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class PayPalController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly string UserId;
        NVPAPICaller payPalCaller = new NVPAPICaller();
        NVPCodec decoder = new NVPCodec();

        public PayPalController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            UserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }        
        

        public IActionResult CheckoutStart()
        {
            //Conversion a USD
            double amtUSD = Math.Round(HttpContext.Session.Get<double>("payment_amt") / GetDolarObservado(),2);


            string retMsg = "";
            string token = "";
            string amt = amtUSD.ToString();

            if (amt != null)
            {
                bool ret = payPalCaller.ShortcutExpressCheckout(amt, ref token, ref retMsg, HttpContext.Session.Get<List<OrderDetail>>("objCasasList"));

                if (ret)
                {
                    HttpContext.Session.Set("token", token);
                    Response.Redirect(retMsg);
                }
                else
                {

                    return RedirectToAction("CheckoutError", new { ErrorMessage = retMsg });

                }
            }
            else
            {
                return RedirectToAction("CheckoutError", new { ErrorCode = "AmtMissing" });
                //Response.Redirect("CheckoutError?ErrorCode=AmtMissing");
            }

            return View();
        }

        public IActionResult CheckoutCancel()
        {
            return View();
        }

        public IActionResult CheckoutError()
        {

            return View();
        }
        public IActionResult CheckoutReview()
        {
            string retMsg = "";
            string token = "";
            string PayerID = "";

            

            token = HttpContext.Session.Get<string>("token");
            int orderId = HttpContext.Session.Get<int>("ordenCompraId");
            var userFromDb = _db.ApplicationUser.Where(u => u.Id == UserId).FirstOrDefault();

            bool ret = payPalCaller.GetCheckoutDetails(token, ref PayerID, ref decoder, ref retMsg);

            if (ret)
            {
                HttpContext.Session.Set("payerId", PayerID);

                var myOrder = _db.Order.Where(o => o.Id == orderId).FirstOrDefault();

                myOrder.OrderDate = Convert.ToDateTime(decoder["TIMESTAMP"].ToString());
                myOrder.UserId = UserId;
                myOrder.FirstName = decoder["FIRSTNAME"].ToString();
                myOrder.LastName = decoder["LASTNAME"].ToString();
                myOrder.Address = decoder["SHIPTOSTREET"].ToString();
                myOrder.City = decoder["SHIPTOCITY"].ToString();
                myOrder.State = decoder["SHIPTOSTATE"].ToString();
                myOrder.PostalCode = decoder["SHIPTOZIP"].ToString();
                myOrder.Country = decoder["SHIPTOCOUNTRYCODE"].ToString();
                myOrder.Email = decoder["EMAIL"].ToString();
                myOrder.Phone = userFromDb.PhoneNumber;
                myOrder.Total = double.Parse(decoder["AMT"]);

                //antiguo arreglo creado para Chile CLP
                // string auxi = decoder["AMT"];
                // string[] partes = auxi.Split('.');
                //double i1 = double.Parse(partes[0]);
                //myOrder.Total = i1;

                // Verify total payment amount as set on CheckoutStart
                try
                {
                    double paymentAmountOnCheckout = Math.Round((HttpContext.Session.Get<double>("payment_amt") / GetDolarObservado()),2);
                    double paymentAmoutFromPayPal = double.Parse(decoder["AMT"]);

                    if (paymentAmountOnCheckout != paymentAmoutFromPayPal)
                    {
                        return RedirectToAction("CheckoutError", new { Desc = "Amount%20total%20mismatch." + " /" + paymentAmountOnCheckout + " /" + paymentAmoutFromPayPal });
                        //Response.Redirect("CheckoutError?" + "Desc=Amount%20total%20mismatch." + " /" + paymentAmountOnCheckout + " /" + paymentAmoutFromPayPal);
                    }
                }
                catch (Exception)
                {
                    return RedirectToAction("CheckoutError", new { Desc = "Amount%20total%20mismatch." + " /" + Math.Round(HttpContext.Session.Get<double>("payment_amt")/GetDolarObservado(),2) + " /" + decoder["AMT"] + "On catch payPal Controller" });
                    //Response.Redirect("CheckoutError?" + "Desc=Amount%20total%20mismatch." + " /" + HttpContext.Session.Get<double>("payment_amt") + " /" + decoder["AMT"] + "On catch payPal Controller");
                }

                _db.SaveChanges();
                HttpContext.Session.Set("userCheckoutCompleted", "true");
                return View(myOrder);
            }
            else
            {
                return RedirectToAction("CheckoutError", new { ErrorMessage = retMsg });
                //Response.Redirect("CheckoutError?" + retMsg);
            }
            //return View();
        }

        public IActionResult CheckoutComplete()
        {
            if (HttpContext.Session.Get<string>("userCheckoutCompleted") != "true")
            {
                HttpContext.Session.Set("userCheckoutCompleted", string.Empty);
                return RedirectToAction("CheckoutError", new { Desc = "Unvalidated%20Checkout." });
                //Response.Redirect("CheckoutError?" + "Desc=Unvalidated%20Checkout.");
            }

            string retMsg = "";
            string token = "";
            string finalPaymentAmount = "";
            string PayerID = "";
            

            token = HttpContext.Session.Get<string>("token");
            PayerID = HttpContext.Session.Get<string>("payerId");
            finalPaymentAmount = HttpContext.Session.Get<string>("payment_amt");

            bool ret = payPalCaller.DoCheckoutPayment(finalPaymentAmount, token, PayerID, ref decoder, ref retMsg);

            if (ret)
            {
                // Retrieve PayPal confirmation value.
                string PaymentConfirmation = decoder["PAYMENTINFO_0_TRANSACTIONID"].ToString();

                int currentOrderId = -1;
                if (HttpContext.Session.Get<int>("ordenCompraId").ToString() != string.Empty)
                {
                    currentOrderId = HttpContext.Session.Get<int>("ordenCompraId");
                }

                if (currentOrderId >= 0)
                {
                    var orderFromDb = _db.Order.Single(o => o.Id == currentOrderId);
                    orderFromDb.PaymentTransactionId = PaymentConfirmation;

                    //Obtener datos del usuario para grabarlos en la tabla contrato
                    /* var userFromdb = _db.ApplicationUser.Where(u => u.Id == UserId).FirstOrDefault();
                     if (userFromdb != null)
                     {
                         var contrato = new Contract();
                         contrato.OrderId = HttpContext.Session.Get<int>("ordenCompraId");
                         contrato.Descripcion = "Fist generation";
                         contrato.Fecha = DateTime.Today;
                         contrato.FechaEntrega = DateTime.Today.AddDays(30);
                         contrato.RazonSocial = userFromdb.RazonSocial;
                         contrato.Rut = userFromdb.Rut;
                         contrato.Email = userFromdb.Email;
                         contrato.Telefono = userFromdb.PhoneNumber;
                         contrato.Total = double.Parse(finalPaymentAmount);
                         contrato.MetodoPago = "PayPal";
                         _db.Contract.Add(contrato);


                     }*/
                    _db.SaveChanges();
                }

                //aca podria limpiar todo(variables de sesion), no?

            }
            else
            {
                return RedirectToAction("CheckoutError", new { ErrorMessage = retMsg });
            }

            var order = _db.Order.Where(o => o.Id == HttpContext.Session.Get<int>("ordenCompraId")).FirstOrDefault();

            return View(order);
        }

        //Obtengo el valor del dolar actualizado
        private double GetDolarObservado()
        {
            string apiUrl = "https://www.mindicador.cl/api";

            WebClient http = new WebClient();

            http.Headers.Add(HttpRequestHeader.Accept, "application/json");

            string jsonString = http.DownloadString(apiUrl);

            JObject rss = JObject.Parse(jsonString);

            double valorDolarObservado = (double)rss["dolar"]["valor"];

            return valorDolarObservado;
        }
    }
}