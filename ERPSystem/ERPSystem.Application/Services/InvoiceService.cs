using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Invoices;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using ERPSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<InvoiceDto>>> GetAllAsync()
        {
            var invoices = await _unitOfWork.Invoices
                .GetQueryable()
                .Include(invoice => invoice.Sale)
                    .ThenInclude(sale => sale.Customer)
                .Include(invoice => invoice.Payments)
                .Select(invoice => new InvoiceDto
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    DueDate = invoice.DueDate,
                    TotalAmount = invoice.TotalAmount,
                    PaidAmount = invoice.Payments
                        .Where(payment => payment.Status == PaymentStatus.Completed)
                        .Sum(payment => payment.Amount),
                    Status = invoice.Status,
                    SaleId = invoice.SaleId,
                    CustomerName = invoice.Sale.Customer.FullName
                })
                .ToListAsync();

            return ResultDto<List<InvoiceDto>>.Success(invoices);
        }

        public async Task<ResultDto<InvoiceDto>> GetByIdAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices
                .GetQueryable()
                .Include(i => i.Sale)
                    .ThenInclude(s => s.Customer)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice is null)
            {
                return ResultDto<InvoiceDto>.Failure("Invoice not found.");
            }

            return ResultDto<InvoiceDto>.Success(MapToDto(invoice));
        }

        public async Task<ResultDto<InvoiceDto>> CreateAsync(CreateInvoiceDto dto)
        {
            var sale = await _unitOfWork.Sales
                .GetQueryable()
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == dto.SaleId);

            if (sale is null)
            {
                return ResultDto<InvoiceDto>.Failure("Sale not found.");
            }

            var existingInvoice = await _unitOfWork.Invoices.GetAsync(i => i.SaleId == dto.SaleId);

            if (existingInvoice is not null)
            {
                return ResultDto<InvoiceDto>.Failure("This sale already has an invoice.");
            }

            if (dto.DueDate < DateTime.UtcNow.Date)
            {
                return ResultDto<InvoiceDto>.Failure("Due date cannot be in the past.");
            }

            var invoice = new Invoice
            {
                SaleId = sale.Id,
                InvoiceNumber = GenerateInvoiceNumber(),
                InvoiceDate = DateTime.UtcNow,
                DueDate = dto.DueDate,
                TotalAmount = sale.TotalAmount,
                Status = InvoiceStatus.Sent
            };

            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            var createdInvoice = await _unitOfWork.Invoices
                .GetQueryable()
                .Include(i => i.Sale)
                    .ThenInclude(s => s.Customer)
                .Include(i => i.Payments)
                .FirstAsync(i => i.Id == invoice.Id);

            return ResultDto<InvoiceDto>.Success(
                MapToDto(createdInvoice),
                "Invoice created successfully.");
        }

        public async Task<ResultDto<InvoiceDto>> UpdateStatusAsync(UpdateInvoiceStatusDto dto)
        {
            var invoice = await _unitOfWork.Invoices
                .GetQueryable()
                .Include(i => i.Sale)
                    .ThenInclude(s => s.Customer)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == dto.InvoiceId);

            if (invoice is null)
            {
                return ResultDto<InvoiceDto>.Failure("Invoice not found.");
            }

            invoice.Status = dto.Status;

            _unitOfWork.Invoices.Update(invoice);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<InvoiceDto>.Success(
                MapToDto(invoice),
                "Invoice status updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);

            if (invoice is null)
            {
                return ResultDto<bool>.Failure("Invoice not found.");
            }

            _unitOfWork.Invoices.Delete(invoice);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Invoice deleted successfully.");
        }

        private static InvoiceDto MapToDto(Invoice invoice)
        {
            var paidAmount = invoice.Payments
                .Where(payment => payment.Status == PaymentStatus.Completed)
                .Sum(payment => payment.Amount);

            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                TotalAmount = invoice.TotalAmount,
                PaidAmount = paidAmount,
                Status = invoice.Status,
                SaleId = invoice.SaleId,
                CustomerName = invoice.Sale?.Customer?.FullName
            };
        }

        private static string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
    }
}
