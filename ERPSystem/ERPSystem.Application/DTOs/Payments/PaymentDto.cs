using ERPSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Payments
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public string? InvoiceNumber { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public PaymentStatus Status { get; set; }
    }
}
