using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Reports;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<DashboardReportDto>> GetDashboardAsync()
        {
            var totalProducts = await _unitOfWork.Products.GetQueryable().CountAsync();
            var totalCustomers = await _unitOfWork.Customers.GetQueryable().CountAsync();
            var totalSuppliers = await _unitOfWork.Suppliers.GetQueryable().CountAsync();
            var totalEmployees = await _unitOfWork.Employees.GetQueryable().CountAsync();

            var totalSales = await _unitOfWork.Sales.GetQueryable().CountAsync();
            var totalSalesAmount = await _unitOfWork.Sales.GetQueryable().SumAsync(sale => sale.TotalAmount);

            var totalPurchases = await _unitOfWork.Purchases.GetQueryable().CountAsync();
            var totalPurchaseAmount = await _unitOfWork.Purchases.GetQueryable().SumAsync(purchase => purchase.TotalAmount);

            var lowStockProductCount = await _unitOfWork.Stocks
                .GetQueryable()
                .CountAsync(stock => stock.Quantity <= stock.MinimumQuantity);

            var totalPaidAmount = await _unitOfWork.Payments
                .GetQueryable()
                .Where(payment => payment.Status == PaymentStatus.Completed)
                .SumAsync(payment => payment.Amount);

            var totalInvoiceAmount = await _unitOfWork.Invoices
                .GetQueryable()
                .Where(invoice => invoice.Status != InvoiceStatus.Cancelled)
                .SumAsync(invoice => invoice.TotalAmount);

            var result = new DashboardReportDto
            {
                TotalProducts = totalProducts,
                TotalCustomers = totalCustomers,
                TotalSuppliers = totalSuppliers,
                TotalEmployees = totalEmployees,
                TotalSales = totalSales,
                TotalSalesAmount = totalSalesAmount,
                TotalPurchases = totalPurchases,
                TotalPurchaseAmount = totalPurchaseAmount,
                LowStockProductCount = lowStockProductCount,
                TotalPaidAmount = totalPaidAmount,
                TotalUnpaidInvoiceAmount = totalInvoiceAmount - totalPaidAmount
            };

            return ResultDto<DashboardReportDto>.Success(result);
        }

        public async Task<ResultDto<List<LowStockReportDto>>> GetLowStockAsync()
        {
            var result = await _unitOfWork.Stocks
                .GetQueryable()
                .Include(stock => stock.Product)
                .Include(stock => stock.Warehouse)
                .Where(stock => stock.Quantity <= stock.MinimumQuantity)
                .Select(stock => new LowStockReportDto
                {
                    StockId = stock.Id,
                    ProductName = stock.Product.Name,
                    WarehouseName = stock.Warehouse.Name,
                    Quantity = stock.Quantity,
                    MinimumQuantity = stock.MinimumQuantity
                })
                .ToListAsync();

            return ResultDto<List<LowStockReportDto>>.Success(result);
        }

        public async Task<ResultDto<List<MonthlySalesReportDto>>> GetMonthlySalesAsync()
        {
            var result = await _unitOfWork.Sales
                .GetQueryable()
                .GroupBy(sale => new
                {
                    sale.SaleDate.Year,
                    sale.SaleDate.Month
                })
                .Select(group => new MonthlySalesReportDto
                {
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    TotalSales = group.Count(),
                    TotalAmount = group.Sum(sale => sale.TotalAmount)
                })
                .OrderBy(report => report.Year)
                .ThenBy(report => report.Month)
                .ToListAsync();

            return ResultDto<List<MonthlySalesReportDto>>.Success(result);
        }
    }
}
