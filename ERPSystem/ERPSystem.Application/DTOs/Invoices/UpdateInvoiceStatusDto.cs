using ERPSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Invoices
{
    public class UpdateInvoiceStatusDto
    {
        public int InvoiceId { get; set; }

        public InvoiceStatus Status { get; set; }
    }
}
