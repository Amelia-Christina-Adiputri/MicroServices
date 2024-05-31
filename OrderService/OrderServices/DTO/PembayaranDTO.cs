using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.DAL
{
    public class PembayaranDTO
    {
        public int CustomerID { get; set; }
        public decimal Nominal { get; set; }
    }
}