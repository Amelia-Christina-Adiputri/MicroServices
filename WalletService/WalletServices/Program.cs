using WalletServices;
using WalletServices.DAL;
using WalletServices.DAL.Interface;
using WalletServices.Models;
using Microsoft.AspNetCore.Mvc;
using WalletServices.DTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICustomer, CustomerDAL>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/customers", (ICustomer customer) => 
{
    return Results.Ok(customer.GetAll());
});


app.MapGet("/customers/{id}", (ICustomer customer, int id) =>
{
    var cust = customer.GetById(id);
    if (cust == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(cust);
});

app.MapGet("/customers/{username}/{password}", (ICustomer customer, string username, string password) =>
{
    var cust = customer.GetCustomer(username, password);
    if (cust == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(cust);
});

app.MapPost("/customers", (ICustomer customer, CreateCustomerDTO obj) =>
{
    try
    {
        var cust = new Customer
        {
            UserName = obj.UserName,
            Password = obj.Password,
            FullName = obj.FullName,
            Saldo = obj.Saldo
        };
        var custWId = customer.Insert(cust);
        return Results.Created($"/customer{cust.CustomerID}", custWId);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/customers/{id}", (ICustomer customer, int id) =>
{
    try
    {
        customer.Delete(id);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/customers", async (ICustomer customer, Customer obj) =>
{
    try
    {
        customer.Update(obj);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/topup/{nominal}", async (ICustomer customer, TopUpDTO topUpDTO) =>
{
    try
    {
        var cus = customer.GetCustomer(topUpDTO.UserName, topUpDTO.Password);
        if(cus == null)
        {
            throw new ArgumentException("Username atau Password Salah");
        }
        cus.Saldo = cus.Saldo + topUpDTO.Nominal;
        customer.Update(cus);
        return Results.Created($"/customers/{cus.CustomerID}", cus);;
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/pembayaran/{id}/{nominal}", async (ICustomer customer, int id, decimal nominal) =>
{
    try
    {
        var cus = customer.GetById(id);
        if(cus == null)
        {
            throw new ArgumentException("Customer tidak ditemukan!");
        }
        cus.Saldo = cus.Saldo - nominal;
        customer.Update(cus);
        return Results.Created($"/customers/{cus.CustomerID}", cus);;
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/pengembalian/{id}/{nominal}", async (ICustomer customer, int id, decimal nominal) =>
{
    try
    {
        var cus = customer.GetById(id);
        if(cus == null)
        {
            throw new ArgumentException("Customer tidak ditemukan!");
        }
        cus.Saldo = cus.Saldo + nominal;
        customer.Update(cus);
        return Results.Created($"/customers/{cus.CustomerID}", cus);;
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.Run();