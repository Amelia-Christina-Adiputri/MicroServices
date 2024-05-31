using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OrderServices.DAL;
using OrderServices.Models;

namespace OrderServices.Services
{
    public class WalletService : IWalletService

    {
        private readonly HttpClient _httpClient;

        // use httpclient
        public WalletService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:7004");
        }

        public async Task<Customer> GetCustomer(string username, string password)
        {
            var response = await _httpClient.GetAsync($"/customers/{username}/{password}");
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var customer = JsonSerializer.Deserialize<Customer>(results);
                if (customer == null)
                {
                    throw new ArgumentException("Username atau Password Salah");
                }
                return customer;
            }
            else
            {
                throw new ArgumentException($"Username atau Password Salah - httpstatus: {response.StatusCode}");
            }
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            var response = await _httpClient.GetAsync($"/customers/{id}");
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var customer = JsonSerializer.Deserialize<Customer>(results);
                if (customer == null)
                {
                    throw new ArgumentException("Customer tidak ditemukan!");
                }
                return customer;
            }
            else
            {
                throw new ArgumentException($"Customer tidak ditemukan - httpstatus: {response.StatusCode}");
            }
        }

        public async Task Pembayaran(PembayaranDTO obj)
        {
            string json = JsonSerializer.Serialize(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/pembayaran/{obj.CustomerID}/{obj.Nominal}", content);
        }
        public async Task Pengembalian(PembayaranDTO obj)
        {
            string json = JsonSerializer.Serialize(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/pengembalian/{obj.CustomerID}/{obj.Nominal}", content);
        }
    }
}