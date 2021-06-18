using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSM.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int BuildingId { get; set; }

        //[Range(1,1)]
        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        [ForeignKey("BuildingId")]
        public virtual Building Building { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        
    }
}
