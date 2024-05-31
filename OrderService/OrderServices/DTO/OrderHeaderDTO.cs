using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.DTO
{
    public class OrderHeaderDTO
    {
        public int OrderHeaderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
    }
}