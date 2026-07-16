using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Employees;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<EmployeeDto>>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employees
                .GetQueryable()
                .Include(employee => employee.Department)
                .Select(employee => new EmployeeDto
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    Position = employee.Position,
                    Salary = employee.Salary,
                    HireDate = employee.HireDate,
                    DepartmentId = employee.DepartmentId,
                    DepartmentName = employee.Department.Name
                })
                .ToListAsync();

            return ResultDto<List<EmployeeDto>>.Success(employees);
        }

        public async Task<ResultDto<EmployeeDto>> GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees
                .GetQueryable()
                .Include(employee => employee.Department)
                .FirstOrDefaultAsync(employee => employee.Id == id);

            if (employee is null)
            {
                return ResultDto<EmployeeDto>.Failure("Employee not found.");
            }

            var result = new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Position = employee.Position,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department.Name
            };

            return ResultDto<EmployeeDto>.Success(result);
        }

        public async Task<ResultDto<EmployeeDto>> CreateAsync(CreateEmployeeDto dto)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);

            if (department is null)
            {
                return ResultDto<EmployeeDto>.Failure("Department not found.");
            }

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Position = dto.Position,
                Salary = dto.Salary,
                HireDate = dto.HireDate,
                DepartmentId = dto.DepartmentId
            };

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            var result = new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Position = employee.Position,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                DepartmentId = employee.DepartmentId,
                DepartmentName = department.Name
            };

            return ResultDto<EmployeeDto>.Success(result, "Employee created successfully.");
        }

        public async Task<ResultDto<EmployeeDto>> UpdateAsync(UpdateEmployeeDto dto)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.Id);

            if (employee is null)
            {
                return ResultDto<EmployeeDto>.Failure("Employee not found.");
            }

            var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);

            if (department is null)
            {
                return ResultDto<EmployeeDto>.Failure("Department not found.");
            }

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.Position = dto.Position;
            employee.Salary = dto.Salary;
            employee.HireDate = dto.HireDate;
            employee.DepartmentId = dto.DepartmentId;

            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();

            var result = new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Position = employee.Position,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                DepartmentId = employee.DepartmentId,
                DepartmentName = department.Name
            };

            return ResultDto<EmployeeDto>.Success(result, "Employee updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee is null)
            {
                return ResultDto<bool>.Failure("Employee not found.");
            }

            _unitOfWork.Employees.Delete(employee);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Employee deleted successfully.");
        }
    }
}
