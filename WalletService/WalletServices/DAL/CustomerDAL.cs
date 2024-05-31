using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WalletServices.DAL.Interface;
using WalletServices.Models;

namespace WalletServices.DAL
{
    public class CustomerDAL : ICustomer
    {
        private readonly IConfiguration _config;
        public CustomerDAL(IConfiguration config)
        {
            _config = config;
        }
        private string GetConnectionString()
        {
            return @"Data Source=.\SQLEXPRESS;Initial Catalog=WalletDb;Integrated Security=True";
        }
        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"DELETE FROM Customers WHERE CustomerId = @CustomerId";
                try
                {
                    conn.Execute(strSql, new {CustomerId = id});
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

        public IEnumerable<Customer> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT * FROM Customers ORDER BY FullName asc";
                var Customers = conn.Query<Customer>(strSql);
                return Customers;
            }
        }

        public Customer GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT * FROM Customers  WHERE CustomerID = @CustomerID";
                var param = new { CustomerID = id };
                var customer = conn.QuerySingleOrDefault<Customer>(strSql,param);
                if(customer == null)
                {
                    throw new ArgumentException("Data tidak ditemukan");
                }
                return customer;
            }  
        }

        public Customer GetCustomer(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT * FROM Customers  WHERE Username = @Username AND Password = @Password;";
                var param = new 
                { 
                    Username = username,
                    Password = password 
                };
                var customer = conn.QuerySingleOrDefault<Customer>(strSql,param);
                if(customer == null)
                {
                    throw new ArgumentException("Data tidak ditemukan");
                }
                return customer;
            }  
        }

        public Customer Insert(Customer obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"INSERT INTO Customers (UserName, Password, FullName, Saldo) VALUES (@UserName, @Password, @FullName, @Saldo); select @@IDENTITY";
                try
                {
                    var param = new 
                    {
                        UserName = obj.UserName,
                        Password = obj.Password,
                        FullName = obj.FullName,
                        Saldo = obj.Saldo
                    };
                    var newId = conn.ExecuteScalar<int>(strSql, param);
                    obj.CustomerID = newId;
                    return obj;
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

        public void Update(Customer obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"UPDATE Customers SET UserName = @UserName, Password = @Password, FullName = @FullName, Saldo = @Saldo WHERE CustomerId = @customerId";
                var param = new 
                {
                    UserName = obj.UserName, 
                    Password = obj.Password, 
                    FullName = obj.FullName, 
                    Saldo = obj.Saldo,
                    CustomerId = obj.CustomerID
                };
                try
                {
                    conn.Execute(strSql, param);
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
