using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices.Models;

namespace CatalogServices.DAL.Interfaces
{
    public interface IProduct : ICrud<Product>
    {
        IEnumerable<Product> GetAllWithCategory();
        void UpdateStockAfterOrder(ProductUpdateStockDto obj);
        void UpdateStockCanceledOrder(ProductUpdateStockDto obj);
    }
}