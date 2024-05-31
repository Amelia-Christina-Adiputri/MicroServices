using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletServices.Models;

namespace WalletServices.DAL.Interface
{
    public interface ICustomer : ICrud<Customer>
    {
        // public void TopUp(Customer obj);
        public Customer GetCustomer(string username, string password);
    }
}