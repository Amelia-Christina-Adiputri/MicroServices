using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices.DAL.Interfaces;
using CatalogServices.Models;
using Dapper;
using System.Data.SqlClient;
using System.Linq.Expressions;


namespace CatalogServices.DAL
{
    public class ProductDAL : IProduct
    {
        private readonly IConfiguration _config;
        public ProductDAL(IConfiguration config)
        {
            _config = config;
        }
        private string GetConnectionString()
        {
            return @"Data Source=.\SQLEXPRESS;Initial Catalog=CatalogDb;Integrated Security=True";
        }   
        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"DELETE FROM Products 
                               WHERE ProductID = @ProductID";
                var param = new { ProductID = id };
                try
                {
                    var product = conn.QuerySingleOrDefault<Product>(strSql,param);
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }

        public IEnumerable<Product> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT * FROM Products ORDER BY Name asc";
                var products = conn.Query<Product>(strSql);
                return products;
            }
        }

        public IEnumerable<Product> GetAllWithCategory()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT p.ProductID, p.Name, p.Description, p.Price, p.CategoryID, p.Quantity, c.CategoryName FROM Products p INNER JOIN Categories c ON p.CategoryID = c.CategoryID ORDER BY p.Name asc";
                var products = conn.Query<Product>(strSql);
                return products;
            }
        }

        public Product GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT p.ProductID, p.Name, p.Description, p.Price, p.CategoryID, p.Quantity, c.CategoryName FROM Products p INNER JOIN Categories c ON p.CategoryID = c.CategoryID WHERE p.ProductID = @ProductID";
                var param = new { ProductID = id };
                var product = conn.QuerySingleOrDefault<Product>(strSql,param);
                if(product == null)
                {
                    throw new ArgumentException("Data tidak ditemukan");
                }
                return product;
            }        
        }

        public void Insert(Product obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"INSERT INTO Products (Name, Description, Price, CategoryID, Quantity) VALUES (@Name, @Description, @Price, @CategoryID, @Quantity)";
                var param = new
                {
                    Name = obj.Name,
                    Description = obj.Description,
                    price = obj.Price,
                    CategoryID = obj.CategoryID,
                    Quantity = obj.Quantity
                };
                try
                {
                    conn.Execute(strSql,param);
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }

        public void Update(Product obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"UPDATE Products SET CategoryID = @CategoryID, Name = @Name, Description = @Description, Price = @Price, Quantity = @Quantity
                            WHERE ProductID = @ProductID";
                var param = new
                {
                    ProductID = obj.ProductID,
                    Name = obj.Name,
                    CategoryID = obj.CategoryID,
                    Description = obj.Description,
                    price = obj.Price,
                    Quantity = obj.Quantity
                };
                try
                {
                    conn.Execute(strSql,param);
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }

        public void UpdateStockAfterOrder(ProductUpdateStockDto obj)
        {
            var strSql = @"UPDATE Products SET Quantity = Quantity -  @Quantity
                            WHERE ProductID = @ProductID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var param = new
                {
                    ProductId = obj.ProductID,
                    Quantity = obj.Quantity
                };
                try
                {
                    conn.Execute(strSql,param);
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }

        public void UpdateStockCanceledOrder(ProductUpdateStockDto obj)
        {
            var strSql = @"UPDATE Products SET Quantity = Quantity +  @Quantity
                            WHERE ProductID = @ProductID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var param = new
                {
                    ProductId = obj.ProductID,
                    Quantity = obj.Quantity
                };
                try
                {
                    conn.Execute(strSql,param);
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }
    }
}