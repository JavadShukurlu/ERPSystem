using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IStockService
    {
        Task<ResultDto<List<StockDto>>> GetAllAsync();

        Task<ResultDto<StockDto>> GetByIdAsync(int id);

        Task<ResultDto<StockDto>> CreateAsync(CreateStockDto dto);

        Task<ResultDto<StockDto>> UpdateAsync(UpdateStockDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);

        Task<ResultDto<StockDto>> IncreaseAsync(AdjustStockDto dto);

        Task<ResultDto<StockDto>> DecreaseAsync(AdjustStockDto dto);

        Task<ResultDto<List<StockDto>>> GetLowStockAsync();
    }
}
