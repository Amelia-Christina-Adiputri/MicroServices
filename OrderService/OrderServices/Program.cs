using OrderServices;
using OrderServices.DAL;
using OrderServices.DAL.Interface;
using OrderServices.DTO;
using OrderServices.Models;
using Microsoft.AspNetCore.Mvc;
using OrderServices.Services;
using OrderServices.DAL;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IOrderHeader, OrderHeaderDAL>();
builder.Services.AddScoped<IOrderDetail, OrderDetailDAL>();

//register HttpClient
builder.Services.AddHttpClient<IProductService, ProductService>().AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(6000)));
builder.Services.AddHttpClient<IWalletService, WalletService>().AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(6000)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/orderHeaders", (IOrderHeader orderHeader) =>
{
    return Results.Ok(orderHeader.GetAll());
});

app.MapGet("/orderHeaders/{id}", (IOrderHeader orderHeader, int id) =>
{
    var order = orderHeader.GetById(id);
    if (order == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(order);
});

app.MapPost("/orderHeaders", async (IOrderHeader orderHeader, IWalletService walletService, OrderHeaderCreateDTO obj) =>
{
    try
    {
        var customer = await walletService.GetCustomer(obj.UserName, obj.Password);
        if (customer == null)
        {
            return Results.BadRequest("Username atau password salah");
        }
        var Order = new OrderHeader
        {
            CustomerID = customer.customerID,
            OrderDate = obj.OrderDate
        };
        var id = orderHeader.Insert(Order);
        return Results.Created($"/orderHeaders/{id}", id);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/orderDetails", (IOrderDetail orderDetail) =>
{
    return Results.Ok(orderDetail.GetAll());
});

app.MapGet("/orderDetails/{id}", (IOrderDetail orderDetail, int id) =>
{
    var order = orderDetail.GetById(id);
    if (order == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(order);
});

app.MapPost("/orderDetails", async (IOrderDetail orderDetail, IOrderHeader orderHeader, IWalletService walletService, IProductService productService, OrderDetailDTO obj)=>
{
    try
    {
        // CEK APAKAH PRODUK ADA
        var product = await productService.GetProductById(obj.ProductId);
        if (product == null)
        {
            return Results.BadRequest("Product not found");
        }

        // CEK APAKAH STOK MENCUKUPI
        if (product.quantity < obj.Quantity)
        {
            return Results.BadRequest("Stock not enough");
        }
        
        // MENGAMBIL DATA ORDERHEADER
        var orhead = orderHeader.GetById(obj.OrderHeaderId);

        
        // CEK APAKAH SALDO MENCUKUPI
        var customer = await walletService.GetCustomerById(orhead.CustomerID);
        var total = obj.Quantity * product.price;
        if (customer.saldo < (total))
        {
            return Results.BadRequest("Saldo anda tidak mencukupi!");
        }

        // MEMBUAT OBJEK ORDERDETAIL
        var OrderDetail = new OrderDetail
        {
            OrderHeaderId = obj.OrderHeaderId,
            ProductId = obj.ProductId,
            Quantity = obj.Quantity,
            Price = product.price
        };

        // MENAMBAHKAN ORDERDETAIL
        var order = orderDetail.Insert(OrderDetail);

        // POTONG SALDO WALLET
        var pembayaran = new PembayaranDTO
        {
            CustomerID = customer.customerID,
            Nominal = total
        };
        await walletService.Pembayaran(pembayaran);

        // UPDATE STOK
        var productUpdateStockDto = new ProductUpdateStockDto
        {
            ProductID = obj.ProductId,
            Quantity = obj.Quantity
        };
        await productService.UpdateProductStock(productUpdateStockDto);

        // MENAMPILKAN ORDERDETAIL YANG DIBUAT
        return Results.Created($"/orderDetails/{orhead}", order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/orderDetail/{id}", async (IOrderDetail orderDetail, IOrderHeader orderHeader, IWalletService walletService, IProductService productService, int id) =>
{
    try
    {
        // MENGAMBIL DATA ORDERDETAIL
        var orDet = orderDetail.GetById(id);

        // MENGAMBIL DATA ORDERHEADER
        var orHead = orderHeader.GetById(orDet.OrderHeaderId);


        // KEMBALIKAN SALDO WALLET
        var customer = await walletService.GetCustomerById(orHead.CustomerID);
        var pengembalian = new PembayaranDTO
        {
            CustomerID = customer.customerID,
            Nominal = orDet.Quantity * orDet.Price
        };
        await walletService.Pengembalian(pengembalian);

        // UPDATE STOK
        var productUpdateStockDto = new ProductUpdateStockDto
        {
            ProductID = orDet.ProductId,
            Quantity = orDet.Quantity
        };
        await productService.UpdateCanceledProductStock(productUpdateStockDto);

        orderDetail.Delete(id);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});


app.Run();