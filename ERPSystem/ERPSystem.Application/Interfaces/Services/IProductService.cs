using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ResultDto<List<ProductDto>>> GetAllAsync();

        Task<ResultDto<ProductDto>> GetByIdAsync(int id);

        Task<ResultDto<ProductDto>> CreateAsync(CreateProductDto dto);

        Task<ResultDto<ProductDto>> UpdateAsync(UpdateProductDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
