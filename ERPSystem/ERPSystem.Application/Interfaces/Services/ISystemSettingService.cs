using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.SystemSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface ISystemSettingService
    {
        Task<ResultDto<SystemSettingDto>> GetAsync();

        Task<ResultDto<SystemSettingDto>> UpdateAsync(UpdateSystemSettingDto dto);
    }
}
