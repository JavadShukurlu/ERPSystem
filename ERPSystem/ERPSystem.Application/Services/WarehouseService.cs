using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Warehouses;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<WarehouseDto>>> GetAllAsync()
        {
            var warehouses = await _unitOfWork.Warehouses.GetAllAsync();

            var result = warehouses.Select(warehouse => new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location
            }).ToList();

            return ResultDto<List<WarehouseDto>>.Success(result);
        }

        public async Task<ResultDto<WarehouseDto>> GetByIdAsync(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);

            if (warehouse is null)
            {
                return ResultDto<WarehouseDto>.Failure("Warehouse not found.");
            }

            var result = new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location
            };

            return ResultDto<WarehouseDto>.Success(result);
        }

        public async Task<ResultDto<WarehouseDto>> CreateAsync(CreateWarehouseDto dto)
        {
            var warehouse = new Warehouse
            {
                Name = dto.Name,
                Location = dto.Location
            };

            await _unitOfWork.Warehouses.AddAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();

            var result = new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location
            };

            return ResultDto<WarehouseDto>.Success(result, "Warehouse created successfully.");
        }

        public async Task<ResultDto<WarehouseDto>> UpdateAsync(UpdateWarehouseDto dto)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(dto.Id);

            if (warehouse is null)
            {
                return ResultDto<WarehouseDto>.Failure("Warehouse not found.");
            }

            warehouse.Name = dto.Name;
            warehouse.Location = dto.Location;

            _unitOfWork.Warehouses.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();

            var result = new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location
            };

            return ResultDto<WarehouseDto>.Success(result, "Warehouse updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);

            if (warehouse is null)
            {
                return ResultDto<bool>.Failure("Warehouse not found.");
            }

            var hasStocks = _unitOfWork.Stocks
                .GetQueryable()
                .Any(stock => stock.WarehouseId == id);

            if (hasStocks)
            {
                return ResultDto<bool>.Failure("This warehouse has stock records. Delete stocks first.");
            }

            _unitOfWork.Warehouses.Delete(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Warehouse deleted successfully.");
        }
    }
}
