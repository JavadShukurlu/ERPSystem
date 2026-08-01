using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Customers;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using ERPSystem.Domain.Enums;

namespace ERPSystem.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private const string ModuleName = "Customers";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IModulePermissionService _modulePermissionService;

        public CustomerService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IModulePermissionService modulePermissionService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _modulePermissionService = modulePermissionService;
        }

        public async Task<ResultDto<List<CustomerDto>>> GetAllAsync()
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<List<CustomerDto>>.Failure("User is not authenticated.");
            }

            var viewPermission = await _modulePermissionService.CheckPermissionAsync(
                userId,
                ModuleName,
                "View");

            if (!viewPermission.Data!.CanAccess)
            {
                return ResultDto<List<CustomerDto>>.Failure("You do not have permission to view customers.");
            }

            var customers = await _unitOfWork.Customers.GetAllAsync();

            if (viewPermission.Data.AccessLevel == PermissionAccessLevel.Own)
            {
                customers = customers
                    .Where(customer => customer.CreatedByUserId == userId)
                    .ToList();
            }

            var result = customers.Select(customer => new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                ImageUrl = customer.ImageUrl,
                CreatedDate = customer.CreatedDate,
                UpdatedDate = customer.UpdatedDate,
                CreatedByUserId = customer.CreatedByUserId,
                UpdatedByUserId = customer.UpdatedByUserId
            }).ToList();

            return ResultDto<List<CustomerDto>>.Success(result);
        }

        public async Task<ResultDto<CustomerDto>> GetByIdAsync(int id)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<CustomerDto>.Failure("User is not authenticated.");
            }

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);

            if (customer is null)
            {
                return ResultDto<CustomerDto>.Failure("Customer not found.");
            }

            var hasPermission = await _modulePermissionService.HasPermissionAsync(
                userId,
                ModuleName,
                "View",
                customer);

            if (!hasPermission)
            {
                return ResultDto<CustomerDto>.Failure("You do not have permission to view this customer.");
            }

            var result = MapToDto(customer);

            return ResultDto<CustomerDto>.Success(result);
        }

        public async Task<ResultDto<CustomerDto>> CreateAsync(CreateCustomerDto dto)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<CustomerDto>.Failure("User is not authenticated.");
            }

            var hasPermission = await _modulePermissionService.HasPermissionAsync(
                userId,
                ModuleName,
                "Create");

            if (!hasPermission)
            {
                return ResultDto<CustomerDto>.Failure("You do not have permission to create customers.");
            }

            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                ImageUrl = dto.ImageUrl
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            var result = MapToDto(customer);

            return ResultDto<CustomerDto>.Success(result, "Customer created successfully.");
        }

        public async Task<ResultDto<CustomerDto>> UpdateAsync(UpdateCustomerDto dto)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<CustomerDto>.Failure("User is not authenticated.");
            }

            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.Id);

            if (customer is null)
            {
                return ResultDto<CustomerDto>.Failure("Customer not found.");
            }

            var hasPermission = await _modulePermissionService.HasPermissionAsync(
                userId,
                ModuleName,
                "Update",
                customer);

            if (!hasPermission)
            {
                return ResultDto<CustomerDto>.Failure("You do not have permission to update this customer.");
            }

            customer.FullName = dto.FullName;
            customer.Email = dto.Email;
            customer.PhoneNumber = dto.PhoneNumber;
            customer.Address = dto.Address;
            customer.ImageUrl = dto.ImageUrl;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            var result = MapToDto(customer);

            return ResultDto<CustomerDto>.Success(result, "Customer updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<bool>.Failure("User is not authenticated.");
            }

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);

            if (customer is null)
            {
                return ResultDto<bool>.Failure("Customer not found.");
            }

            var hasPermission = await _modulePermissionService.HasPermissionAsync(
                userId,
                ModuleName,
                "Delete",
                customer);

            if (!hasPermission)
            {
                return ResultDto<bool>.Failure("You do not have permission to delete this customer.");
            }

            _unitOfWork.Customers.Delete(customer);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Customer deleted successfully.");
        }

        private static CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                ImageUrl = customer.ImageUrl,
                CreatedDate = customer.CreatedDate,
                UpdatedDate = customer.UpdatedDate,
                CreatedByUserId = customer.CreatedByUserId,
                UpdatedByUserId = customer.UpdatedByUserId
            };
        }
    }
}