using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using OrderServices.DAL.Interface;
using OrderServices.Models;

namespace OrderServices.DAL
{
    public class OrderDetailDAL : IOrderDetail
    {
        private readonly IConfiguration _config;
        public OrderDetailDAL(IConfiguration config)
        {
            _config = config;
        }
        private string GetConnectionString()
        {
            return @"Data Source=.\SQLEXPRESS;Initial Catalog=OrderDb;Integrated Security=True";
        }
        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"DELETE FROM OrderDetail 
                               WHERE OrderDetailId = @OrderDetailId";
                var param = new { OrderDetailId = id };
                try
                {
                    var orderDetail = conn.QuerySingleOrDefault<OrderDetail>(strSql,param);
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

        IEnumerable<OrderDetail> ICrud<OrderDetail>.GetAll()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT * FROM OrderDetail  ORDER BY OrderHeaderID asc";
                var OrderDetail = conn.Query<OrderDetail>(strSql);
                return OrderDetail;
            }
        }

        public OrderDetail GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT * FROM OrderDetail WHERE OrderDetailId = @OrderDetailId";
                    var OrderDetail = conn.QuerySingleOrDefault<OrderDetail>(strSql, new {OrderDetailId = id});
                    if(OrderDetail == null)
                    {
                        throw new ArgumentException("Data tidak ditemukan");
                    }
                        return OrderDetail;
            }
        }

        public OrderDetail Insert(OrderDetail obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"INSERT INTO OrderDetail (OrderHeaderId, ProductId, Quantity, Price) VALUES (@OrderHeaderId, @ProductId, @Quantity, @Price); select @@IDENTITY";
                var param = new {OrderHeaderId = obj.OrderHeaderId, ProductId = obj.ProductId, Quantity = obj.Quantity, Price = obj.Price};
                try
                {
                    var newId = conn.ExecuteScalar<int>(strSql, param);
                    obj.OrderDetailId = newId;
                    return obj;
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException(sqlEx.Message);
                }
            }
        }

        public void Update(OrderDetail obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"UPDATE OrderDetail SET OrderHeaderId = @OrderHeaderId, ProductId = @ProductId, Quantity = @Quantity, Price = @Price";
                var param = new {OrderHeaderId = obj.OrderHeaderId, ProductId = obj.ProductId, Quantity = obj.Quantity, Price = obj.Price};
                try
                {
                    var newId = conn.ExecuteScalar<int>(strSql, param);
                    obj.OrderDetailId = newId;
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException(sqlEx.Message);
                }
            }
        }
    }
}