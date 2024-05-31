using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletServices.DTO
{
    public class TopUpDTO
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public decimal Nominal { get; set; }
    }
}