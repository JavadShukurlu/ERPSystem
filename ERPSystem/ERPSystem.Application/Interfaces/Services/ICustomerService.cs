using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<ResultDto<List<CustomerDto>>> GetAllAsync();

        Task<ResultDto<CustomerDto>> GetByIdAsync(int id);

        Task<ResultDto<CustomerDto>> CreateAsync(CreateCustomerDto dto);

        Task<ResultDto<CustomerDto>> UpdateAsync(UpdateCustomerDto dto);

        Task<ResultDto<bool>> DeleteAsync(int id);
    }
}
