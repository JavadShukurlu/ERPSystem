using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.AuditLogs
{
    public class CreateAuditLogDto
    {
        public string UserId { get; set; } = null!;

        public string Action { get; set; } = null!;

        public string EntityName { get; set; } = null!;

        public int? EntityId { get; set; }

        public string? Details { get; set; }
    }
}
