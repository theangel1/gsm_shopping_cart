using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSM.Models
{
    public class Order
    {
        //validaciones al final
        public int Id { get; set; }

        [Display(Name = "Fecha Orden Compra")]
        public DateTime OrderDate { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [Display(Name = "Ciudad")]
        public string City { get; set; }

        [Display(Name = "Estado")]
        public string State { get; set; }

        [Display(Name = "Código Postal")]
        public string PostalCode { get; set; }

        [Display(Name = "País")]
        public string Country { get; set; }

        [Display(Name = "Teléfono")]
        public string Phone { get; set; }

        
        public string Email { get; set; }
        public double Total { get; set; }

        [Display(Name = "Id Transacción")]
        public string PaymentTransactionId { get; set; }
        public bool HasBeenShipped { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual Contract Contract { get; set; }
    }
}
