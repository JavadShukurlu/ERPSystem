using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface ISupplierService
    {
        Task<ResultDto<List<SupplierDto>>> GetAllAsync();

        Task<ResultDto<SupplierDto>> GetByIdAsync(int id);

        Task<ResultDto<SupplierDto>> CreateAsync(CreateSupplierDto dto);

        Task<ResultDto<SupplierDto>> UpdateAsync(UpdateSupplierDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
