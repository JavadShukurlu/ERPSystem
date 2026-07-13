using ERPSystem.Application.DTOs.Categories;
using ERPSystem.Application.DTOs.Common;
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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();

            var result = categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            }).ToList();

            return ResultDto<List<CategoryDto>>.Success(result);
        }

        public async Task<ResultDto<CategoryDto>> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category is null)
            {
                return ResultDto<CategoryDto>.Failure("Category not found.");
            }

            var result = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return ResultDto<CategoryDto>.Success(result);
        }

        public async Task<ResultDto<CategoryDto>> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var result = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return ResultDto<CategoryDto>.Success(result, "Category created successfully.");
        }

        public async Task<ResultDto<CategoryDto>> UpdateAsync(UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id);

            if (category is null)
            {
                return ResultDto<CategoryDto>.Failure("Category not found.");
            }

            category.Name = dto.Name;
            category.Description = dto.Description;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            var result = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return ResultDto<CategoryDto>.Success(result, "Category updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category is null)
            {
                return ResultDto<bool>.Failure("Category not found.");
            }

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Category deleted successfully.");
        }
    }
}
