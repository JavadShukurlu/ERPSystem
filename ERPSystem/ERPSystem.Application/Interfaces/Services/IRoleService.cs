using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ResultDto<List<RoleDto>>> GetAllAsync();

        Task<ResultDto<RoleDto>> CreateAsync(CreateRoleDto dto);

        Task<ResultDto<bool>> DeleteAsync(string id);
    }
}
