using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSM.Models.ViewModel
{
    public class OrdenDetalleViewModel
    {
        public List<OrderDetail> OrderDetail { get; set; }
        public Order Order { get; set; }
    }
}
