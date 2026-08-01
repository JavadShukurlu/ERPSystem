using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Products;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;

namespace ERPSystem.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();

            var result = products.Select(product => new ProductDto
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
                UpdatedDate = product.UpdatedDate
            }).ToList();

            return ResultDto<List<ProductDto>>.Success(result);
        }

        public async Task<ResultDto<ProductDto>> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product is null)
            {
                return ResultDto<ProductDto>.Failure("Product not found.");
            }

            var result = new ProductDto
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
                UpdatedDate = product.UpdatedDate
            };

            return ResultDto<ProductDto>.Success(result);
        }

        public async Task<ResultDto<ProductDto>> CreateAsync(CreateProductDto dto)
        {
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

            var result = new ProductDto
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
                UpdatedDate = product.UpdatedDate
            };

            return ResultDto<ProductDto>.Success(result, "Product created successfully.");
        }

        public async Task<ResultDto<ProductDto>> UpdateAsync(UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);

            if (product is null)
            {
                return ResultDto<ProductDto>.Failure("Product not found.");
            }

            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.Description = dto.Description;
            product.ImageUrl = dto.ImageUrl;
            product.PurchasePrice = dto.PurchasePrice;
            product.SalePrice = dto.SalePrice;
            product.CategoryId = dto.CategoryId;
            product.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var result = new ProductDto
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
                UpdatedDate = product.UpdatedDate
            };

            return ResultDto<ProductDto>.Success(result, "Product updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product is null)
            {
                return ResultDto<bool>.Failure("Product not found.");
            }

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Product deleted successfully.");
        }
    }
}