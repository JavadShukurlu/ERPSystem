using ERPSystem.Domain.Common;
using ERPSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Domain.Entities
{
    public class Payment:BaseEntity
    {
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string PaymentMethod { get; set; } = null!;

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; } = null!;
    }
}
