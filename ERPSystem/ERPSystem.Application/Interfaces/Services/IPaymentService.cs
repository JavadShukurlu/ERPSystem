using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<ResultDto<List<PaymentDto>>> GetAllAsync();

        Task<ResultDto<PaymentDto>> GetByIdAsync(int id);

        Task<ResultDto<PaymentDto>> CreateAsync(CreatePaymentDto dto);

        Task<ResultDto<PaymentDto>> UpdateStatusAsync(UpdatePaymentStatusDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
