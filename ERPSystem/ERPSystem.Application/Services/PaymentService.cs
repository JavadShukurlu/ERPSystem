using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Payments;
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
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<PaymentDto>>> GetAllAsync()
        {
            var payments = await _unitOfWork.Payments
                .GetQueryable()
                .Include(payment => payment.Invoice)
                .Select(payment => new PaymentDto
                {
                    Id = payment.Id,
                    InvoiceId = payment.InvoiceId,
                    InvoiceNumber = payment.Invoice.InvoiceNumber,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    PaymentMethod = payment.PaymentMethod,
                    Status = payment.Status
                })
                .ToListAsync();

            return ResultDto<List<PaymentDto>>.Success(payments);
        }

        public async Task<ResultDto<PaymentDto>> GetByIdAsync(int id)
        {
            var payment = await _unitOfWork.Payments
                .GetQueryable()
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment is null)
            {
                return ResultDto<PaymentDto>.Failure("Payment not found.");
            }

            return ResultDto<PaymentDto>.Success(MapToDto(payment));
        }

        public async Task<ResultDto<PaymentDto>> CreateAsync(CreatePaymentDto dto)
        {
            if (dto.Amount <= 0)
            {
                return ResultDto<PaymentDto>.Failure("Payment amount must be greater than zero.");
            }

            var invoice = await _unitOfWork.Invoices
                .GetQueryable()
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == dto.InvoiceId);

            if (invoice is null)
            {
                return ResultDto<PaymentDto>.Failure("Invoice not found.");
            }

            if (invoice.Status == InvoiceStatus.Cancelled)
            {
                return ResultDto<PaymentDto>.Failure("Cannot add payment to cancelled invoice.");
            }

            var paidAmount = invoice.Payments
                .Where(payment => payment.Status == PaymentStatus.Completed)
                .Sum(payment => payment.Amount);

            var remainingAmount = invoice.TotalAmount - paidAmount;

            if (dto.Amount > remainingAmount)
            {
                return ResultDto<PaymentDto>.Failure($"Payment amount exceeds remaining amount. Remaining: {remainingAmount}.");
            }

            var payment = new Payment
            {
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethod = dto.PaymentMethod,
                Status = PaymentStatus.Completed
            };

            await _unitOfWork.Payments.AddAsync(payment);

            if (dto.Amount == remainingAmount)
            {
                invoice.Status = InvoiceStatus.Paid;
            }
            else
            {
                invoice.Status = InvoiceStatus.Sent;
            }

            _unitOfWork.Invoices.Update(invoice);

            await _unitOfWork.SaveChangesAsync();

            var createdPayment = await _unitOfWork.Payments
                .GetQueryable()
                .Include(p => p.Invoice)
                .FirstAsync(p => p.Id == payment.Id);

            return ResultDto<PaymentDto>.Success(
                MapToDto(createdPayment),
                "Payment created successfully.");
        }

        public async Task<ResultDto<PaymentDto>> UpdateStatusAsync(UpdatePaymentStatusDto dto)
        {
            var payment = await _unitOfWork.Payments
                .GetQueryable()
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Payments)
                .FirstOrDefaultAsync(p => p.Id == dto.PaymentId);

            if (payment is null)
            {
                return ResultDto<PaymentDto>.Failure("Payment not found.");
            }

            payment.Status = dto.Status;

            var invoice = payment.Invoice;

            var paidAmount = invoice.Payments
                .Where(p => p.Id == payment.Id ? dto.Status == PaymentStatus.Completed : p.Status == PaymentStatus.Completed)
                .Sum(p => p.Id == payment.Id && dto.Status != PaymentStatus.Completed ? 0 : p.Amount);

            invoice.Status = paidAmount >= invoice.TotalAmount
                ? InvoiceStatus.Paid
                : InvoiceStatus.Sent;

            _unitOfWork.Payments.Update(payment);
            _unitOfWork.Invoices.Update(invoice);

            await _unitOfWork.SaveChangesAsync();

            return ResultDto<PaymentDto>.Success(
                MapToDto(payment),
                "Payment status updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);

            if (payment is null)
            {
                return ResultDto<bool>.Failure("Payment not found.");
            }

            _unitOfWork.Payments.Delete(payment);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Payment deleted successfully.");
        }

        private static PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                InvoiceId = payment.InvoiceId,
                InvoiceNumber = payment.Invoice?.InvoiceNumber,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status
            };
        }
    }
}
