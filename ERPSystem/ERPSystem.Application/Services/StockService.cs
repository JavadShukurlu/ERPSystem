using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Stocks;
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
    public class StockService : IStockService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<StockDto>>> GetAllAsync()
        {
            var stocks = await _unitOfWork.Stocks
                .GetQueryable()
                .Include(stock => stock.Product)
                .Include(stock => stock.Warehouse)
                .Select(stock => new StockDto
                {
                    Id = stock.Id,
                    ProductId = stock.ProductId,
                    ProductName = stock.Product.Name,
                    WarehouseId = stock.WarehouseId,
                    WarehouseName = stock.Warehouse.Name,
                    Quantity = stock.Quantity,
                    MinimumQuantity = stock.MinimumQuantity
                })
                .ToListAsync();

            return ResultDto<List<StockDto>>.Success(stocks);
        }

        public async Task<ResultDto<StockDto>> GetByIdAsync(int id)
        {
            var stock = await _unitOfWork.Stocks
                .GetQueryable()
                .Include(stock => stock.Product)
                .Include(stock => stock.Warehouse)
                .FirstOrDefaultAsync(stock => stock.Id == id);

            if (stock is null)
            {
                return ResultDto<StockDto>.Failure("Stock not found.");
            }

            var result = MapToDto(stock);

            return ResultDto<StockDto>.Success(result);
        }

        public async Task<ResultDto<StockDto>> CreateAsync(CreateStockDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);

            if (product is null)
            {
                return ResultDto<StockDto>.Failure("Product not found.");
            }

            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(dto.WarehouseId);

            if (warehouse is null)
            {
                return ResultDto<StockDto>.Failure("Warehouse not found.");
            }

            var existingStock = await _unitOfWork.Stocks.GetAsync(stock =>
                stock.ProductId == dto.ProductId &&
                stock.WarehouseId == dto.WarehouseId);

            if (existingStock is not null)
            {
                return ResultDto<StockDto>.Failure("This product already has a stock record in this warehouse.");
            }

            if (dto.Quantity < 0)
            {
                return ResultDto<StockDto>.Failure("Quantity cannot be negative.");
            }

            if (dto.MinimumQuantity < 0)
            {
                return ResultDto<StockDto>.Failure("Minimum quantity cannot be negative.");
            }

            var stock = new Stock
            {
                ProductId = dto.ProductId,
                WarehouseId = dto.WarehouseId,
                Quantity = dto.Quantity,
                MinimumQuantity = dto.MinimumQuantity
            };

            await _unitOfWork.Stocks.AddAsync(stock);
            await _unitOfWork.SaveChangesAsync();

            var result = new StockDto
            {
                Id = stock.Id,
                ProductId = stock.ProductId,
                ProductName = product.Name,
                WarehouseId = stock.WarehouseId,
                WarehouseName = warehouse.Name,
                Quantity = stock.Quantity,
                MinimumQuantity = stock.MinimumQuantity
            };

            return ResultDto<StockDto>.Success(result, "Stock created successfully.");
        }

        public async Task<ResultDto<StockDto>> UpdateAsync(UpdateStockDto dto)
        {
            var stock = await _unitOfWork.Stocks.GetByIdAsync(dto.Id);

            if (stock is null)
            {
                return ResultDto<StockDto>.Failure("Stock not found.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);

            if (product is null)
            {
                return ResultDto<StockDto>.Failure("Product not found.");
            }

            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(dto.WarehouseId);

            if (warehouse is null)
            {
                return ResultDto<StockDto>.Failure("Warehouse not found.");
            }

            var duplicateStock = await _unitOfWork.Stocks.GetAsync(existingStock =>
                existingStock.Id != dto.Id &&
                existingStock.ProductId == dto.ProductId &&
                existingStock.WarehouseId == dto.WarehouseId);

            if (duplicateStock is not null)
            {
                return ResultDto<StockDto>.Failure("This product already has a stock record in this warehouse.");
            }

            if (dto.Quantity < 0)
            {
                return ResultDto<StockDto>.Failure("Quantity cannot be negative.");
            }

            if (dto.MinimumQuantity < 0)
            {
                return ResultDto<StockDto>.Failure("Minimum quantity cannot be negative.");
            }

            stock.ProductId = dto.ProductId;
            stock.WarehouseId = dto.WarehouseId;
            stock.Quantity = dto.Quantity;
            stock.MinimumQuantity = dto.MinimumQuantity;

            _unitOfWork.Stocks.Update(stock);
            await _unitOfWork.SaveChangesAsync();

            var result = new StockDto
            {
                Id = stock.Id,
                ProductId = stock.ProductId,
                ProductName = product.Name,
                WarehouseId = stock.WarehouseId,
                WarehouseName = warehouse.Name,
                Quantity = stock.Quantity,
                MinimumQuantity = stock.MinimumQuantity
            };

            return ResultDto<StockDto>.Success(result, "Stock updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var stock = await _unitOfWork.Stocks.GetByIdAsync(id);

            if (stock is null)
            {
                return ResultDto<bool>.Failure("Stock not found.");
            }

            _unitOfWork.Stocks.Delete(stock);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Stock deleted successfully.");
        }

        public async Task<ResultDto<StockDto>> IncreaseAsync(AdjustStockDto dto)
        {
            if (dto.Quantity <= 0)
            {
                return ResultDto<StockDto>.Failure("Quantity must be greater than zero.");
            }

            var stock = await _unitOfWork.Stocks
                .GetQueryable()
                .Include(stock => stock.Product)
                .Include(stock => stock.Warehouse)
                .FirstOrDefaultAsync(stock => stock.Id == dto.StockId);

            if (stock is null)
            {
                return ResultDto<StockDto>.Failure("Stock not found.");
            }

            stock.Quantity += dto.Quantity;

            _unitOfWork.Stocks.Update(stock);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<StockDto>.Success(MapToDto(stock), "Stock increased successfully.");
        }

        public async Task<ResultDto<StockDto>> DecreaseAsync(AdjustStockDto dto)
        {
            if (dto.Quantity <= 0)
            {
                return ResultDto<StockDto>.Failure("Quantity must be greater than zero.");
            }

            var stock = await _unitOfWork.Stocks
                .GetQueryable()
                .Include(stock => stock.Product)
                .Include(stock => stock.Warehouse)
                .FirstOrDefaultAsync(stock => stock.Id == dto.StockId);

            if (stock is null)
            {
                return ResultDto<StockDto>.Failure("Stock not found.");
            }

            if (stock.Quantity < dto.Quantity)
            {
                return ResultDto<StockDto>.Failure("Insufficient stock quantity.");
            }

            stock.Quantity -= dto.Quantity;

            _unitOfWork.Stocks.Update(stock);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<StockDto>.Success(MapToDto(stock), "Stock decreased successfully.");
        }

        public async Task<ResultDto<List<StockDto>>> GetLowStockAsync()
        {
            var stocks = await _unitOfWork.Stocks
                .GetQueryable()
                .Include(stock => stock.Product)
                .Include(stock => stock.Warehouse)
                .Where(stock => stock.Quantity <= stock.MinimumQuantity)
                .Select(stock => new StockDto
                {
                    Id = stock.Id,
                    ProductId = stock.ProductId,
                    ProductName = stock.Product.Name,
                    WarehouseId = stock.WarehouseId,
                    WarehouseName = stock.Warehouse.Name,
                    Quantity = stock.Quantity,
                    MinimumQuantity = stock.MinimumQuantity
                })
                .ToListAsync();

            return ResultDto<List<StockDto>>.Success(stocks);
        }

        private static StockDto MapToDto(Stock stock)
        {
            return new StockDto
            {
                Id = stock.Id,
                ProductId = stock.ProductId,
                ProductName = stock.Product?.Name,
                WarehouseId = stock.WarehouseId,
                WarehouseName = stock.Warehouse?.Name,
                Quantity = stock.Quantity,
                MinimumQuantity = stock.MinimumQuantity
            };
        }
    }
}
