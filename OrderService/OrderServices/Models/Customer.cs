using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.Models
{
    public class Customer
    {
        public int customerID { get; set; }
        public string? userName { get; set; }
        public string? password { get; set; }
        public string? fullName { get; set; }
        public decimal saldo { get; set; }
    }
}