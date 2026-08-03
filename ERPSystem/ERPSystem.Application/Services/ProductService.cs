using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Products;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using ERPSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ERPSystem.Application.Services
{
    public class ProductService : IProductService
    {
        private const string ModuleName = "Products";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IModulePermissionService _modulePermissionService;
        private readonly UserManager<AppUser> _userManager;

        public ProductService(
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

        public async Task<ResultDto<List<ProductDto>>> GetAllAsync()
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<List<ProductDto>>.Failure("User is not authenticated.");
            }

            var isAdmin = await IsAdminAsync(userId);

            var products = await _unitOfWork.Products.GetAllAsync();

            if (!isAdmin)
            {
                var viewPermission = await _modulePermissionService.CheckPermissionAsync(
                    userId,
                    ModuleName,
                    "View");

                if (viewPermission.Data is null || !viewPermission.Data.CanAccess)
                {
                    return ResultDto<List<ProductDto>>.Failure("You do not have permission to view products.");
                }

                if (viewPermission.Data.AccessLevel == PermissionAccessLevel.Own)
                {
                    products = products
                        .Where(product => product.CreatedByUserId == userId)
                        .ToList();
                }
            }

            var result = products.Select(MapToDto).ToList();

            return ResultDto<List<ProductDto>>.Success(result);
        }

        public async Task<ResultDto<ProductDto>> GetByIdAsync(int id)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<ProductDto>.Failure("User is not authenticated.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product is null)
            {
                return ResultDto<ProductDto>.Failure("Product not found.");
            }

            var isAdmin = await IsAdminAsync(userId);

            if (!isAdmin)
            {
                var hasPermission = await _modulePermissionService.HasPermissionAsync(
                    userId,
                    ModuleName,
                    "View",
                    product);

                if (!hasPermission)
                {
                    return ResultDto<ProductDto>.Failure("You do not have permission to view this product.");
                }
            }

            return ResultDto<ProductDto>.Success(MapToDto(product));
        }

        public async Task<ResultDto<ProductDto>> CreateAsync(CreateProductDto dto)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<ProductDto>.Failure("User is not authenticated.");
            }

            var isAdmin = await IsAdminAsync(userId);

            if (!isAdmin)
            {
                var hasPermission = await _modulePermissionService.HasPermissionAsync(
                    userId,
                    ModuleName,
                    "Create");

                if (!hasPermission)
                {
                    return ResultDto<ProductDto>.Failure("You do not have permission to create products.");
                }
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);

            if (category is null)
            {
                return ResultDto<ProductDto>.Failure("Category not found.");
            }

            var product = new Product
            {
                Name = dto.Name,
                SKU = dto.SKU,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                PurchasePrice = dto.PurchasePrice,
                SalePrice = dto.SalePrice,
                CategoryId = dto.CategoryId
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<ProductDto>.Success(MapToDto(product), "Product created successfully.");
        }

        public async Task<ResultDto<ProductDto>> UpdateAsync(UpdateProductDto dto)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<ProductDto>.Failure("User is not authenticated.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);

            if (product is null)
            {
                return ResultDto<ProductDto>.Failure("Product not found.");
            }

            var isAdmin = await IsAdminAsync(userId);

            if (!isAdmin)
            {
                var hasPermission = await _modulePermissionService.HasPermissionAsync(
                    userId,
                    ModuleName,
                    "Update",
                    product);

                if (!hasPermission)
                {
                    return ResultDto<ProductDto>.Failure("You do not have permission to update this product.");
                }
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);

            if (category is null)
            {
                return ResultDto<ProductDto>.Failure("Category not found.");
            }

            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.Description = dto.Description;
            product.ImageUrl = dto.ImageUrl;
            product.PurchasePrice = dto.PurchasePrice;
            product.SalePrice = dto.SalePrice;
            product.CategoryId = dto.CategoryId;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<ProductDto>.Success(MapToDto(product), "Product updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResultDto<bool>.Failure("User is not authenticated.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product is null)
            {
                return ResultDto<bool>.Failure("Product not found.");
            }

            var isAdmin = await IsAdminAsync(userId);

            if (!isAdmin)
            {
                var hasPermission = await _modulePermissionService.HasPermissionAsync(
                    userId,
                    ModuleName,
                    "Delete",
                    product);

                if (!hasPermission)
                {
                    return ResultDto<bool>.Failure("You do not have permission to delete this product.");
                }
            }

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Product deleted successfully.");
        }

        private async Task<bool> IsAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                PurchasePrice = product.PurchasePrice,
                SalePrice = product.SalePrice,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
                CreatedByUserId = product.CreatedByUserId,
                UpdatedByUserId = product.UpdatedByUserId
            };
        }
    }
}