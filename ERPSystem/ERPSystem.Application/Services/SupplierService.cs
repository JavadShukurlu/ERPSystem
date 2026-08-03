using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Suppliers;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ERPSystem.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private const string ModuleName = "Suppliers";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IModulePermissionService _modulePermissionService;
        private readonly UserManager<AppUser> _userManager;

        public SupplierService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IModulePermissionService modulePermissionService,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _modulePermissionService = modulePermissionService;
            _userManager = userManager;
        }

        public async Task<ResultDto<List<SupplierDto>>> GetAllAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();

            if (await IsCurrentUserAdminAsync())
            {
                var adminResult = suppliers
                    .Select(MapToDto)
                    .ToList();

                return ResultDto<List<SupplierDto>>.Success(adminResult);
            }

            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<List<SupplierDto>>.Failure("User not found.");
            }

            var visibleSuppliers = new List<Supplier>();

            foreach (var supplier in suppliers)
            {
                var canView = await _modulePermissionService.HasPermissionAsync(
                    userId,
                    ModuleName,
                    "View",
                    supplier);

                if (canView)
                {
                    visibleSuppliers.Add(supplier);
                }
            }

            var result = visibleSuppliers
                .Select(MapToDto)
                .ToList();

            return ResultDto<List<SupplierDto>>.Success(result);
        }

        public async Task<ResultDto<SupplierDto>> GetByIdAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);

            if (supplier is null)
            {
                return ResultDto<SupplierDto>.Failure("Supplier not found.");
            }

            if (!await CanAccessAsync("View", supplier))
            {
                return ResultDto<SupplierDto>.Failure("You do not have permission to view this supplier.");
            }

            return ResultDto<SupplierDto>.Success(MapToDto(supplier));
        }

        public async Task<ResultDto<SupplierDto>> CreateAsync(CreateSupplierDto dto)
        {
            if (!await CanAccessAsync("Create"))
            {
                return ResultDto<SupplierDto>.Failure("You do not have permission to create suppliers.");
            }

            var supplier = new Supplier
            {
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                LogoUrl = dto.LogoUrl
            };

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<SupplierDto>.Success(
                MapToDto(supplier),
                "Supplier created successfully.");
        }

        public async Task<ResultDto<SupplierDto>> UpdateAsync(UpdateSupplierDto dto)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.Id);

            if (supplier is null)
            {
                return ResultDto<SupplierDto>.Failure("Supplier not found.");
            }

            if (!await CanAccessAsync("Update", supplier))
            {
                return ResultDto<SupplierDto>.Failure("You do not have permission to update this supplier.");
            }

            supplier.CompanyName = dto.CompanyName;
            supplier.ContactName = dto.ContactName;
            supplier.Email = dto.Email;
            supplier.PhoneNumber = dto.PhoneNumber;
            supplier.Address = dto.Address;
            supplier.LogoUrl = dto.LogoUrl;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<SupplierDto>.Success(
                MapToDto(supplier),
                "Supplier updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);

            if (supplier is null)
            {
                return ResultDto<bool>.Failure("Supplier not found.");
            }

            if (!await CanAccessAsync("Delete", supplier))
            {
                return ResultDto<bool>.Failure("You do not have permission to delete this supplier.");
            }

            _unitOfWork.Suppliers.Delete(supplier);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Supplier deleted successfully.");
        }

        private async Task<bool> CanAccessAsync(string actionName, Supplier? supplier = null)
        {
            if (await IsCurrentUserAdminAsync())
            {
                return true;
            }

            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            return await _modulePermissionService.HasPermissionAsync(
                userId,
                ModuleName,
                actionName,
                supplier);
        }

        private async Task<bool> IsCurrentUserAdminAsync()
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        private static SupplierDto MapToDto(Supplier supplier)
        {
            return new SupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Email = supplier.Email,
                PhoneNumber = supplier.PhoneNumber,
                Address = supplier.Address,
                LogoUrl = supplier.LogoUrl,
                CreatedDate = supplier.CreatedDate,
                UpdatedDate = supplier.UpdatedDate,
                CreatedByUserId = supplier.CreatedByUserId,
                UpdatedByUserId = supplier.UpdatedByUserId
            };
        }
    }
}