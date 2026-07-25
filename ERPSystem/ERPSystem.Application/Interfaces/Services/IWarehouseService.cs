using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IWarehouseService
    {
        Task<ResultDto<List<WarehouseDto>>> GetAllAsync();

        Task<ResultDto<WarehouseDto>> GetByIdAsync(int id);

        Task<ResultDto<WarehouseDto>> CreateAsync(CreateWarehouseDto dto);

        Task<ResultDto<WarehouseDto>> UpdateAsync(UpdateWarehouseDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
