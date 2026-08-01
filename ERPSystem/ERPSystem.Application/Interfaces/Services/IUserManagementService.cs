using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IUserManagementService
    {
        Task<ResultDto<List<UserDto>>> GetAllAsync();

        Task<ResultDto<UserDto>> GetByIdAsync(string id);

        Task<ResultDto<UserDto>> CreateAsync(CreateUserDto dto);

        Task<ResultDto<UserDto>> AssignRolesAsync(AssignUserRolesDto dto);

        Task<ResultDto<bool>> DeleteAsync(string id);
    }
}
