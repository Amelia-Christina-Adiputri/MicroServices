using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.DAL;
using OrderServices.Models;

namespace OrderServices.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        Task UpdateProductStock(ProductUpdateStockDto productUpdateStockDto);
        Task UpdateCanceledProductStock(ProductUpdateStockDto productUpdateStockDto);
    }
}