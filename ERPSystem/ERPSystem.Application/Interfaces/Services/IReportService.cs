using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<ResultDto<DashboardReportDto>> GetDashboardAsync();

        Task<ResultDto<List<LowStockReportDto>>> GetLowStockAsync();

        Task<ResultDto<List<MonthlySalesReportDto>>> GetMonthlySalesAsync();
    }
}
