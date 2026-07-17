using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Customers;
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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<CustomerDto>>> GetAllAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();

            var result = customers.Select(customer => new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address
            }).ToList();

            return ResultDto<List<CustomerDto>>.Success(result);
        }

        public async Task<ResultDto<CustomerDto>> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);

            if (customer is null)
            {
                return ResultDto<CustomerDto>.Failure("Customer not found.");
            }

            var result = new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address
            };

            return ResultDto<CustomerDto>.Success(result);
        }

        public async Task<ResultDto<CustomerDto>> CreateAsync(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            var result = new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address
            };

            return ResultDto<CustomerDto>.Success(result, "Customer created successfully.");
        }

        public async Task<ResultDto<CustomerDto>> UpdateAsync(UpdateCustomerDto dto)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.Id);

            if (customer is null)
            {
                return ResultDto<CustomerDto>.Failure("Customer not found.");
            }

            customer.FullName = dto.FullName;
            customer.Email = dto.Email;
            customer.PhoneNumber = dto.PhoneNumber;
            customer.Address = dto.Address;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            var result = new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address
            };

            return ResultDto<CustomerDto>.Success(result, "Customer updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);

            if (customer is null)
            {
                return ResultDto<bool>.Failure("Customer not found.");
            }

            _unitOfWork.Customers.Delete(customer);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Customer deleted successfully.");
        }
    }
}
