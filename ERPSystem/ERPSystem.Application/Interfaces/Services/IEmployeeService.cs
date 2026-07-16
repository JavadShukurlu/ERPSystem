using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<ResultDto<List<EmployeeDto>>> GetAllAsync();

        Task<ResultDto<EmployeeDto>> GetByIdAsync(int id);

        Task<ResultDto<EmployeeDto>> CreateAsync(CreateEmployeeDto dto);

        Task<ResultDto<EmployeeDto>> UpdateAsync(UpdateEmployeeDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
