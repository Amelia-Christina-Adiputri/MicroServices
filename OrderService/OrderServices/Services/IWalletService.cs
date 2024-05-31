using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.DAL;
using OrderServices.Models;

namespace OrderServices.Services
{
    public interface IWalletService
    {
        Task<Customer> GetCustomer(string username, string password);
        Task<Customer> GetCustomerById(int id);
        Task Pembayaran(PembayaranDTO obj);
        Task Pengembalian(PembayaranDTO obj);
    }
}