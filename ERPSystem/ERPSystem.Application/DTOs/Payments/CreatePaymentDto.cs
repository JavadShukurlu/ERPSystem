using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Payments
{
    public class CreatePaymentDto
    {
        public int InvoiceId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string PaymentMethod { get; set; } = null!;
    }
}
