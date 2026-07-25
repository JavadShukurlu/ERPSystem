using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Purchases;
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
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<PurchaseDto>>> GetAllAsync()
        {
            var purchases = await _unitOfWork.Purchases
                .GetQueryable()
                .Include(purchase => purchase.Supplier)
                .Include(purchase => purchase.PurchaseItems)
                    .ThenInclude(item => item.Product)
                .Include(purchase => purchase.PurchaseItems)
                    .ThenInclude(item => item.Warehouse)
                .Select(purchase => new PurchaseDto
                {
                    Id = purchase.Id,
                    PurchaseDate = purchase.PurchaseDate,
                    SupplierId = purchase.SupplierId,
                    SupplierName = purchase.Supplier.CompanyName,
                    TotalAmount = purchase.TotalAmount,
                    Items = purchase.PurchaseItems.Select(item => new PurchaseItemDto
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

            return ResultDto<List<PurchaseDto>>.Success(purchases);
        }

        public async Task<ResultDto<PurchaseDto>> GetByIdAsync(int id)
        {
            var purchase = await _unitOfWork.Purchases
                .GetQueryable()
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Product)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Warehouse)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase is null)
            {
                return ResultDto<PurchaseDto>.Failure("Purchase not found.");
            }

            return ResultDto<PurchaseDto>.Success(MapToDto(purchase));
        }

        public async Task<ResultDto<PurchaseDto>> CreateAsync(CreatePurchaseDto dto)
        {
            if (dto.Items.Count == 0)
            {
                return ResultDto<PurchaseDto>.Failure("Purchase must contain at least one item.");
            }

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId);

            if (supplier is null)
            {
                return ResultDto<PurchaseDto>.Failure("Supplier not found.");
            }

            var purchase = new Purchase
            {
                SupplierId = dto.SupplierId,
                PurchaseDate = dto.PurchaseDate,
                TotalAmount = 0
            };

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                {
                    return ResultDto<PurchaseDto>.Failure("Item quantity must be greater than zero.");
                }

                if (itemDto.UnitPrice <= 0)
                {
                    return ResultDto<PurchaseDto>.Failure("Item unit price must be greater than zero.");
                }

                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);

                if (product is null)
                {
                    return ResultDto<PurchaseDto>.Failure($"Product with id {itemDto.ProductId} not found.");
                }

                var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(itemDto.WarehouseId);

                if (warehouse is null)
                {
                    return ResultDto<PurchaseDto>.Failure($"Warehouse with id {itemDto.WarehouseId} not found.");
                }

                var totalPrice = itemDto.Quantity * itemDto.UnitPrice;

                var purchaseItem = new PurchaseItem
                {
                    ProductId = itemDto.ProductId,
                    WarehouseId = itemDto.WarehouseId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = totalPrice
                };

                purchase.PurchaseItems.Add(purchaseItem);
                purchase.TotalAmount += totalPrice;

                var stock = await _unitOfWork.Stocks.GetAsync(s =>
                    s.ProductId == itemDto.ProductId &&
                    s.WarehouseId == itemDto.WarehouseId);

                if (stock is null)
                {
                    stock = new Stock
                    {
                        ProductId = itemDto.ProductId,
                        WarehouseId = itemDto.WarehouseId,
                        Quantity = itemDto.Quantity,
                        MinimumQuantity = 0
                    };

                    await _unitOfWork.Stocks.AddAsync(stock);
                }
                else
                {
                    stock.Quantity += itemDto.Quantity;
                    _unitOfWork.Stocks.Update(stock);
                }
            }

            await _unitOfWork.Purchases.AddAsync(purchase);
            await _unitOfWork.SaveChangesAsync();

            var createdPurchase = await _unitOfWork.Purchases
                .GetQueryable()
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Product)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Warehouse)
                .FirstAsync(p => p.Id == purchase.Id);

            return ResultDto<PurchaseDto>.Success(
                MapToDto(createdPurchase),
                "Purchase created successfully and stock increased.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var purchase = await _unitOfWork.Purchases.GetByIdAsync(id);

            if (purchase is null)
            {
                return ResultDto<bool>.Failure("Purchase not found.");
            }

            _unitOfWork.Purchases.Delete(purchase);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Purchase deleted successfully.");
        }

        private static PurchaseDto MapToDto(Purchase purchase)
        {
            return new PurchaseDto
            {
                Id = purchase.Id,
                PurchaseDate = purchase.PurchaseDate,
                SupplierId = purchase.SupplierId,
                SupplierName = purchase.Supplier?.CompanyName,
                TotalAmount = purchase.TotalAmount,
                Items = purchase.PurchaseItems.Select(item => new PurchaseItemDto
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
