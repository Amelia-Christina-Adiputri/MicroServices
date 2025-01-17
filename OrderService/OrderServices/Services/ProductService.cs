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
    public class ProductService : IProductService

    {
        private readonly HttpClient _httpClient;

        // use httpclient
        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:7002");
        }
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var response = await _httpClient.GetAsync("api/products");
            if(response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<IEnumerable<Product>>(results);
                if (products == null)
                {
                    throw new ArgumentException("Cannot get products");
                }
                return products;
            }
            else
            {
                throw new ArgumentException($"Cannot get products - httpstatus: {response.StatusCode}");
            }
        }

        public async Task<Product> GetProductById(int id)
        {
            var response = await _httpClient.GetAsync($"/api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(results);
                if (product == null)
                {
                    throw new ArgumentException("Cannot get product");
                }
                return product;
            }
            else
            {
                throw new ArgumentException($"Cannot get product - httpstatus: {response.StatusCode}");
            }
        }

        public async Task UpdateProductStock(ProductUpdateStockDto productUpdateStockDto)
        {
            var json = JsonSerializer.Serialize(productUpdateStockDto);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/api/products/updatestock", data);
            if(!response.IsSuccessStatusCode)
            {
                throw new ArgumentException($"Cannot update product stock - httpstatus: {response.StatusCode}");
            }
        }

        public async Task UpdateCanceledProductStock(ProductUpdateStockDto productUpdateStockDto)
        {
            var json = JsonSerializer.Serialize(productUpdateStockDto);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/api/products/updatecanceledproduct", data);
            if(!response.IsSuccessStatusCode)
            {
                throw new ArgumentException($"Cannot update product stock - httpstatus: {response.StatusCode}");
            }
        }
            
    }
}