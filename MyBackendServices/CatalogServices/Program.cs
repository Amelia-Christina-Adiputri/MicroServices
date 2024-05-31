using System.Security.Cryptography;
using CatalogServices;
using CatalogServices.DAL;
using CatalogServices.DAL.Interfaces;
using CatalogServices.DTO;
using CatalogServices.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//meregister service menggunakan DI
builder.Services.AddScoped<ICategory, CategoryDapper>();
builder.Services.AddScoped<IProduct, ProductDAL>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/categories", (ICategory categoryDal) =>
{
    List<CategoryDTO> categoriesDto = new List<CategoryDTO>();
    var categories = categoryDal.GetAll();
    foreach (var category in categories)
    {
        categoriesDto.Add(new CategoryDTO
        {
            CategoryName = category.CategoryName
        });
    }
    return Results.Ok(categoriesDto);
});

app.MapGet("/api/categories/{id}", (ICategory categoryDal, int id) =>
{
    CategoryDTO categoryDto = new CategoryDTO();
    var category = categoryDal.GetById(id);
    if (category == null)
    {
        return Results.NotFound();
    }
    categoryDto.CategoryName = category.CategoryName;
    return Results.Ok(categoryDto);
});

app.MapGet("/api/categories/search/{name}", (ICategory categoryDal, string name) =>
{
    List<CategoryDTO> categoriesDto = new List<CategoryDTO>();
    var categories = categoryDal.GetByName(name);
    foreach (var category in categories)
    {
        categoriesDto.Add(new CategoryDTO
        {
            CategoryName = category.CategoryName
        });
    }
    return Results.Ok(categoriesDto);
});

app.MapPost("/api/categories", (ICategory categoryDal, CategoryCreateDto categoryCreateDto) =>
{
    try
    {
        Category category = new Category
        {
            CategoryName = categoryCreateDto.CategoryName
        };
        categoryDal.Insert(category);

        //return 201 Created
        return Results.Created($"/api/categories/{category.CategoryID}", category);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/api/categories", (ICategory categoryDal, CategoryUpdateDto categoryUpdateDto) =>
{
    try
    {
        var category = new Category
        {
            CategoryID = categoryUpdateDto.CategoryID,
            CategoryName = categoryUpdateDto.CategoryName
        };
        categoryDal.Update(category);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/api/categories/{id}", (ICategory categoryDal, int id) =>
{
    try
    {
        categoryDal.Delete(id);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/api/products", (IProduct productDal) =>
{
    List<ProductDTO> productsDto = new List<ProductDTO>();
    var products = productDal.GetAll();
    foreach (var product in products)
    {
        productsDto.Add(new ProductDTO
        {
            ProductID = product.ProductID,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        });
    }
    return Results.Ok(productsDto);
});

app.MapGet("/api/productsWCategory", (IProduct productDal) =>
{
    List<ProductDTO> productsDto = new List<ProductDTO>();
    var products = productDal.GetAllWithCategory();
    foreach (var product in products)
    {
        productsDto.Add(new ProductDTO
        {
            ProductID = product.ProductID,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryName = product.CategoryName,
            Quantity = product.Quantity
        });
    }
    return Results.Ok(productsDto);
});

app.MapGet("/api/products/{id}", (IProduct productDal, int id) =>
{
    ProductDTO productDto = new ProductDTO();
    var product = productDal.GetById(id);
    if (product == null)
    {
        return Results.NotFound();
    }
    productDto.ProductID = product.ProductID;
    productDto.Name = product.Name;
    productDto.Description = product.Description;
    productDto.Price = product.Price;
    productDto.CategoryName = product.CategoryName;
    productDto.Quantity = product.Quantity;
    return Results.Ok(productDto);
});

app.MapPost("/api/products", (IProduct productDal, ProductCreateDto productCreateDto) =>
{
    try
    {
        Product product = new Product
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            Price = productCreateDto.Price,
            CategoryID = productCreateDto.CategoryID,
            Quantity = productCreateDto.Quantity
        };
        productDal.Insert(product);

        var productDto = new ProductDTO
        {
            ProductID = product.ProductID,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryName = product.CategoryName,
            Quantity = product.Quantity

        };

        //return 201 Created
        return Results.Created($"/api/products/{product.ProductID}", productDto);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/api/products", (IProduct productDal, ProductUpdateDto productUpdateDto) =>
{
    try
    {
        var product = new Product
        {
            ProductID = productUpdateDto.ProductID,
            CategoryID = productUpdateDto.CategoryID,
            Name = productUpdateDto.Name,
            Description = productUpdateDto.Description,
            Price = productUpdateDto.Price,
            Quantity = productUpdateDto.Quantity
        };
        productDal.Update(product);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/api/products/{id}", (IProduct productDal, int id) =>
{
    try
    {
        productDal.Delete(id);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/api/products/updatestock", (IProduct productDal, ProductUpdateStockDto productUpdateStockDto)=>
{
    try
    {
        productDal.UpdateStockAfterOrder(productUpdateStockDto);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/api/products/updatecanceledproduct", (IProduct productDal, ProductUpdateStockDto productUpdateStockDto)=>
{
    try
    {
        productDal.UpdateStockCanceledOrder(productUpdateStockDto);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
app.Run();
