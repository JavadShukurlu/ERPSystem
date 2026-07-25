using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IPurchaseService
    {
        Task<ResultDto<List<PurchaseDto>>> GetAllAsync();

        Task<ResultDto<PurchaseDto>> GetByIdAsync(int id);

        Task<ResultDto<PurchaseDto>> CreateAsync(CreatePurchaseDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
