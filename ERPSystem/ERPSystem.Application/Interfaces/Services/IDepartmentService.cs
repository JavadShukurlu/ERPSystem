using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<ResultDto<List<DepartmentDto>>> GetAllAsync();

        Task<ResultDto<DepartmentDto>> GetByIdAsync(int id);

        Task<ResultDto<DepartmentDto>> CreateAsync(CreateDepartmentDto dto);

        Task<ResultDto<DepartmentDto>> UpdateAsync(UpdateDepartmentDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
