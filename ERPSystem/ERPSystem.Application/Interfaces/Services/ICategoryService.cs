using ERPSystem.Application.DTOs.Categories;
using ERPSystem.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<ResultDto<List<CategoryDto>>> GetAllAsync();

        Task<ResultDto<CategoryDto>> GetByIdAsync(int id);

        Task<ResultDto<CategoryDto>> CreateAsync(CreateCategoryDto dto);

        Task<ResultDto<CategoryDto>> UpdateAsync(UpdateCategoryDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
