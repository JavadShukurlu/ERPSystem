using ERPSystem.Application.DTOs.AuditLogs;
using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<AuditLogDto>>> GetAllAsync()
        {
            var logs = await _unitOfWork.AuditLogs.GetAllAsync();

            var result = logs
                .OrderByDescending(log => log.ActionDate)
                .Select(log => new AuditLogDto
                {
                    Id = log.Id,
                    UserId = log.UserId,
                    Action = log.Action,
                    EntityName = log.EntityName,
                    EntityId = log.EntityId,
                    ActionDate = log.ActionDate,
                    Details = log.Details
                })
                .ToList();

            return ResultDto<List<AuditLogDto>>.Success(result);
        }

        public async Task<ResultDto<AuditLogDto>> CreateAsync(CreateAuditLogDto dto)
        {
            var log = new AuditLog
            {
                UserId = dto.UserId,
                Action = dto.Action,
                EntityName = dto.EntityName,
                EntityId = dto.EntityId,
                ActionDate = DateTime.UtcNow,
                Details = dto.Details
            };

            await _unitOfWork.AuditLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            var result = new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                ActionDate = log.ActionDate,
                Details = log.Details
            };

            return ResultDto<AuditLogDto>.Success(result, "Audit log created successfully.");
        }
    }
}
