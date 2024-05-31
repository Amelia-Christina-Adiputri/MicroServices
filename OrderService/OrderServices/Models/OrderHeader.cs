using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.Models
{
    public class OrderHeader
    {
        public int OrderHeaderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
    }
}