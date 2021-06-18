using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GSM.Models
{
    public class Contract
    {        
        public int Id { get; set; }

        [ForeignKey("Order"), Display(Name = "Folio")]
        public int OrderId { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaEntrega { get; set; }
        
        public string RazonSocial { get; set; }
        
        public string Rut { get; set; }
        
        public string Email { get; set; }
        
        public string Telefono { get; set; }
        
        public double Total { get; set; }

        
        public string NombreArchivo { get; set; }


        public virtual Order Order { get; set; }

    }
}
