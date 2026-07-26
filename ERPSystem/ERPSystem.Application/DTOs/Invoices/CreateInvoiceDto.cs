using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Invoices
{
    public class CreateInvoiceDto
    {
        public int SaleId { get; set; }

        public DateTime DueDate { get; set; }
    }
}
