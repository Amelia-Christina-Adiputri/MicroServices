using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogServices.DAL
{
    public class ProductUpdateStockDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}