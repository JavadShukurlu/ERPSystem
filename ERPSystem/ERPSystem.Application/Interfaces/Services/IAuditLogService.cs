using ERPSystem.Application.DTOs.AuditLogs;
using ERPSystem.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IAuditLogService
    {
        Task<ResultDto<List<AuditLogDto>>> GetAllAsync();

        Task<ResultDto<AuditLogDto>> CreateAsync(CreateAuditLogDto dto);
    }
}
