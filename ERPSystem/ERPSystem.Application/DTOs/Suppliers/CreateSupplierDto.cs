using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Suppliers
{
    public class CreateSupplierDto
    {
        public string CompanyName { get; set; } = null!;

        public string ContactName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
