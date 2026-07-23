using ERPSystem.Application.DTOs.Auth;
using ERPSystem.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ResultDto<AuthResponseDto>> RegisterAsync(RegisterDto dto);

        Task<ResultDto<AuthResponseDto>> LoginAsync(LoginDto dto);
    }
}
