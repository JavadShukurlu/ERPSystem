using ERPSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Payments
{
    public class UpdatePaymentStatusDto
    {
        public int PaymentId { get; set; }

        public PaymentStatus Status { get; set; }
    }
}
