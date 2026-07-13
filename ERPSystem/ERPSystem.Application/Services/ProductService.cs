using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Products;
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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _unitOfWork.Products
                .GetQueryable()
                .Include(product => product.Category)
                .Select(product => new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    SKU = product.SKU,
                    Description = product.Description,
                    PurchasePrice = product.PurchasePrice,
                    SalePrice = product.SalePrice,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name
                })
                .ToListAsync();

            return ResultDto<List<ProductDto>>.Success(products);
        }

        public async Task<ResultDto<ProductDto>> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products
                .GetQueryable()
                .Include(product => product.Category)
                .FirstOrDefaultAsync(product => product.Id == id);

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
                PurchasePrice = product.PurchasePrice,
                SalePrice = product.SalePrice,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name
            };

            return ResultDto<ProductDto>.Success(result);
        }

        public async Task<ResultDto<ProductDto>> CreateAsync(CreateProductDto dto)
        {
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
                PurchasePrice = product.PurchasePrice,
                SalePrice = product.SalePrice,
                CategoryId = product.CategoryId,
                CategoryName = category.Name
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

            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);

            if (category is null)
            {
                return ResultDto<ProductDto>.Failure("Category not found.");
            }

            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.Description = dto.Description;
            product.PurchasePrice = dto.PurchasePrice;
            product.SalePrice = dto.SalePrice;
            product.CategoryId = dto.CategoryId;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var result = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                PurchasePrice = product.PurchasePrice,
                SalePrice = product.SalePrice,
                CategoryId = product.CategoryId,
                CategoryName = category.Name
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
