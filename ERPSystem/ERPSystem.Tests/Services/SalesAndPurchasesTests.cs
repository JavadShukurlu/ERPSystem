using ERPSystem.Application.DTOs.Purchases;
using ERPSystem.Application.DTOs.Sales;
using ERPSystem.Application.Services;
using ERPSystem.Domain.Entities;
using ERPSystem.Persistence.Context;
using ERPSystem.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Tests.Services
{
    public class SalesAndPurchasesTests
    {
        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Purchase_Should_Increase_Stock()
        {
            var context = CreateDbContext();

            var category = new Category { Name = "Electronics" };
            var product = new Product
            {
                Name = "Laptop",
                SKU = "LP-001",
                PurchasePrice = 1000,
                SalePrice = 1500,
                Category = category
            };
            var warehouse = new Warehouse { Name = "Main Warehouse", Location = "Baku" };
            var supplier = new Supplier
            {
                CompanyName = "Tech Supply",
                ContactName = "Nigar",
                Email = "supplier@test.com"
            };
            var stock = new Stock
            {
                Product = product,
                Warehouse = warehouse,
                Quantity = 50,
                MinimumQuantity = 10
            };

            context.AddRange(category, product, warehouse, supplier, stock);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var service = new PurchaseService(unitOfWork);

            var dto = new CreatePurchaseDto
            {
                SupplierId = supplier.Id,
                Items =
                [
                    new CreatePurchaseItemDto
                {
                    ProductId = product.Id,
                    WarehouseId = warehouse.Id,
                    Quantity = 30,
                    UnitPrice = 1000
                }
                ]
            };

            var result = await service.CreateAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Data!.TotalAmount.Should().Be(30000);

            var updatedStock = await context.Stocks.FirstAsync();
            updatedStock.Quantity.Should().Be(80);
        }

        [Fact]
        public async Task Sale_Should_Decrease_Stock_When_Stock_Is_Available()
        {
            var context = CreateDbContext();

            var category = new Category { Name = "Electronics" };
            var product = new Product
            {
                Name = "Laptop",
                SKU = "LP-001",
                PurchasePrice = 1000,
                SalePrice = 1500,
                Category = category
            };
            var warehouse = new Warehouse { Name = "Main Warehouse", Location = "Baku" };
            var customer = new Customer
            {
                FullName = "Ali Mammadov",
                Email = "ali@test.com"
            };
            var stock = new Stock
            {
                Product = product,
                Warehouse = warehouse,
                Quantity = 80,
                MinimumQuantity = 10
            };

            context.AddRange(category, product, warehouse, customer, stock);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var service = new SaleService(unitOfWork);

            var dto = new CreateSaleDto
            {
                CustomerId = customer.Id,
                Items =
                [
                    new CreateSaleItemDto
                {
                    ProductId = product.Id,
                    WarehouseId = warehouse.Id,
                    Quantity = 20,
                    UnitPrice = 1500
                }
                ]
            };

            var result = await service.CreateAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Data!.TotalAmount.Should().Be(30000);

            var updatedStock = await context.Stocks.FirstAsync();
            updatedStock.Quantity.Should().Be(60);
        }

        [Fact]
        public async Task Sale_Should_Fail_When_Stock_Is_Insufficient()
        {
            var context = CreateDbContext();

            var category = new Category { Name = "Electronics" };
            var product = new Product
            {
                Name = "Laptop",
                SKU = "LP-001",
                PurchasePrice = 1000,
                SalePrice = 1500,
                Category = category
            };
            var warehouse = new Warehouse { Name = "Main Warehouse", Location = "Baku" };
            var customer = new Customer
            {
                FullName = "Ali Mammadov",
                Email = "ali@test.com"
            };
            var stock = new Stock
            {
                Product = product,
                Warehouse = warehouse,
                Quantity = 5,
                MinimumQuantity = 10
            };

            context.AddRange(category, product, warehouse, customer, stock);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var service = new SaleService(unitOfWork);

            var dto = new CreateSaleDto
            {
                CustomerId = customer.Id,
                Items =
                [
                    new CreateSaleItemDto
                {
                    ProductId = product.Id,
                    WarehouseId = warehouse.Id,
                    Quantity = 20,
                    UnitPrice = 1500
                }
                ]
            };

            var result = await service.CreateAsync(dto);

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Insufficient stock");

            var unchangedStock = await context.Stocks.FirstAsync();
            unchangedStock.Quantity.Should().Be(5);
        }

        [Fact]
        public async Task Sale_Should_Fail_When_Items_Are_Empty()
        {
            var context = CreateDbContext();

            var customer = new Customer
            {
                FullName = "Ali Mammadov",
                Email = "ali@test.com"
            };

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var service = new SaleService(unitOfWork);

            var dto = new CreateSaleDto
            {
                CustomerId = customer.Id,
                Items = []
            };

            var result = await service.CreateAsync(dto);

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Sale must contain at least one item.");
        }
    }
}
