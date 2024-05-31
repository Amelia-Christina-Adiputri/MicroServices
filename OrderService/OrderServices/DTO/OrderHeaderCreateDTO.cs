using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.DTO
{
    public class OrderHeaderCreateDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime OrderDate { get; set; }
    }
}