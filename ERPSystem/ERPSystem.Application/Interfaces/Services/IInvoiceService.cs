using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IInvoiceService
    {
        Task<ResultDto<List<InvoiceDto>>> GetAllAsync();

        Task<ResultDto<InvoiceDto>> GetByIdAsync(int id);

        Task<ResultDto<InvoiceDto>> CreateAsync(CreateInvoiceDto dto);

        Task<ResultDto<InvoiceDto>> UpdateStatusAsync(UpdateInvoiceStatusDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
