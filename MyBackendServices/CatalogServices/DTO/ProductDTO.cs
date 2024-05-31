using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogServices.DTO
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public int Quantity { get; set; }
    }
}