using ERPSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Domain.Entities
{
    public class AuditLog:BaseEntity
    {
        public string UserId { get; set; } = null!;

        public string Action { get; set; } = null!;

        public string EntityName { get; set; } = null!;

        public int? EntityId { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        public string? Details { get; set; }
    }
}
