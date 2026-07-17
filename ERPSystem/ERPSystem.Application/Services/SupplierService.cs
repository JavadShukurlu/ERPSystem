using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Suppliers;
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
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<SupplierDto>>> GetAllAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();

            var result = suppliers.Select(supplier => new SupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Email = supplier.Email,
                PhoneNumber = supplier.PhoneNumber,
                Address = supplier.Address
            }).ToList();

            return ResultDto<List<SupplierDto>>.Success(result);
        }

        public async Task<ResultDto<SupplierDto>> GetByIdAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);

            if (supplier is null)
            {
                return ResultDto<SupplierDto>.Failure("Supplier not found.");
            }

            var result = new SupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Email = supplier.Email,
                PhoneNumber = supplier.PhoneNumber,
                Address = supplier.Address
            };

            return ResultDto<SupplierDto>.Success(result);
        }

        public async Task<ResultDto<SupplierDto>> CreateAsync(CreateSupplierDto dto)
        {
            var supplier = new Supplier
            {
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address
            };

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            var result = new SupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Email = supplier.Email,
                PhoneNumber = supplier.PhoneNumber,
                Address = supplier.Address
            };

            return ResultDto<SupplierDto>.Success(result, "Supplier created successfully.");
        }

        public async Task<ResultDto<SupplierDto>> UpdateAsync(UpdateSupplierDto dto)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.Id);

            if (supplier is null)
            {
                return ResultDto<SupplierDto>.Failure("Supplier not found.");
            }

            supplier.CompanyName = dto.CompanyName;
            supplier.ContactName = dto.ContactName;
            supplier.Email = dto.Email;
            supplier.PhoneNumber = dto.PhoneNumber;
            supplier.Address = dto.Address;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            var result = new SupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Email = supplier.Email,
                PhoneNumber = supplier.PhoneNumber,
                Address = supplier.Address
            };

            return ResultDto<SupplierDto>.Success(result, "Supplier updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);

            if (supplier is null)
            {
                return ResultDto<bool>.Failure("Supplier not found.");
            }

            _unitOfWork.Suppliers.Delete(supplier);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Supplier deleted successfully.");
        }
    }
}
