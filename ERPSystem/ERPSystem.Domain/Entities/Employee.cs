using ERPSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Domain.Entities
{
    public class Employee:BaseEntity
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string Position { get; set; } = null!;

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!;
    }
}
