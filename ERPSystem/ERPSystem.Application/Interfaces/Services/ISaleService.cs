using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface ISaleService
    {
        Task<ResultDto<List<SaleDto>>> GetAllAsync();

        Task<ResultDto<SaleDto>> GetByIdAsync(int id);

        Task<ResultDto<SaleDto>> CreateAsync(CreateSaleDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
