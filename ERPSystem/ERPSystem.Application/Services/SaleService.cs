using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Sales;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<SaleDto>>> GetAllAsync()
        {
            var sales = await _unitOfWork.Sales
                .GetQueryable()
                .Include(sale => sale.Customer)
                .Include(sale => sale.SaleItems)
                    .ThenInclude(item => item.Product)
                .Include(sale => sale.SaleItems)
                    .ThenInclude(item => item.Warehouse)
                .Select(sale => new SaleDto
                {
                    Id = sale.Id,
                    SaleDate = sale.SaleDate,
                    CustomerId = sale.CustomerId,
                    CustomerName = sale.Customer.FullName,
                    TotalAmount = sale.TotalAmount,
                    Items = sale.SaleItems.Select(item => new SaleItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        WarehouseId = item.WarehouseId,
                        WarehouseName = item.Warehouse.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                })
                .ToListAsync();

            return ResultDto<List<SaleDto>>.Success(sales);
        }

        public async Task<ResultDto<SaleDto>> GetByIdAsync(int id)
        {
            var sale = await _unitOfWork.Sales
                .GetQueryable()
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(i => i.Product)
                .Include(s => s.SaleItems)
                    .ThenInclude(i => i.Warehouse)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale is null)
            {
                return ResultDto<SaleDto>.Failure("Sale not found.");
            }

            return ResultDto<SaleDto>.Success(MapToDto(sale));
        }

        public async Task<ResultDto<SaleDto>> CreateAsync(CreateSaleDto dto)
        {
            if (dto.Items.Count == 0)
            {
                return ResultDto<SaleDto>.Failure("Sale must contain at least one item.");
            }

            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);

            if (customer is null)
            {
                return ResultDto<SaleDto>.Failure("Customer not found.");
            }

            var sale = new Sale
            {
                CustomerId = dto.CustomerId,
                SaleDate = dto.SaleDate,
                TotalAmount = 0
            };

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                {
                    return ResultDto<SaleDto>.Failure("Item quantity must be greater than zero.");
                }

                if (itemDto.UnitPrice <= 0)
                {
                    return ResultDto<SaleDto>.Failure("Item unit price must be greater than zero.");
                }

                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);

                if (product is null)
                {
                    return ResultDto<SaleDto>.Failure($"Product with id {itemDto.ProductId} not found.");
                }

                var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(itemDto.WarehouseId);

                if (warehouse is null)
                {
                    return ResultDto<SaleDto>.Failure($"Warehouse with id {itemDto.WarehouseId} not found.");
                }

                var stock = await _unitOfWork.Stocks.GetAsync(s =>
                    s.ProductId == itemDto.ProductId &&
                    s.WarehouseId == itemDto.WarehouseId);

                if (stock is null)
                {
                    return ResultDto<SaleDto>.Failure(
                        $"No stock found for product id {itemDto.ProductId} in warehouse id {itemDto.WarehouseId}.");
                }

                if (stock.Quantity < itemDto.Quantity)
                {
                    return ResultDto<SaleDto>.Failure(
                        $"Insufficient stock for product id {itemDto.ProductId}. Available quantity: {stock.Quantity}.");
                }

                var totalPrice = itemDto.Quantity * itemDto.UnitPrice;

                var saleItem = new SaleItem
                {
                    ProductId = itemDto.ProductId,
                    WarehouseId = itemDto.WarehouseId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = totalPrice
                };

                sale.SaleItems.Add(saleItem);
                sale.TotalAmount += totalPrice;

                stock.Quantity -= itemDto.Quantity;
                _unitOfWork.Stocks.Update(stock);
            }

            await _unitOfWork.Sales.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();

            var createdSale = await _unitOfWork.Sales
                .GetQueryable()
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(i => i.Product)
                .Include(s => s.SaleItems)
                    .ThenInclude(i => i.Warehouse)
                .FirstAsync(s => s.Id == sale.Id);

            return ResultDto<SaleDto>.Success(
                MapToDto(createdSale),
                "Sale created successfully and stock decreased.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var sale = await _unitOfWork.Sales.GetByIdAsync(id);

            if (sale is null)
            {
                return ResultDto<bool>.Failure("Sale not found.");
            }

            _unitOfWork.Sales.Delete(sale);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Sale deleted successfully.");
        }

        private static SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                SaleDate = sale.SaleDate,
                CustomerId = sale.CustomerId,
                CustomerName = sale.Customer?.FullName,
                TotalAmount = sale.TotalAmount,
                Items = sale.SaleItems.Select(item => new SaleItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name,
                    WarehouseId = item.WarehouseId,
                    WarehouseName = item.Warehouse?.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };
        }
    }
}
