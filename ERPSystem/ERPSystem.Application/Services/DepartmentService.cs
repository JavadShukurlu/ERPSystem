using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Departments;
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
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<DepartmentDto>>> GetAllAsync()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();

            var result = departments.Select(department => new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            }).ToList();

            return ResultDto<List<DepartmentDto>>.Success(result);
        }

        public async Task<ResultDto<DepartmentDto>> GetByIdAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);

            if (department is null)
            {
                return ResultDto<DepartmentDto>.Failure("Department not found.");
            }

            var result = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };

            return ResultDto<DepartmentDto>.Success(result);
        }

        public async Task<ResultDto<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
        {
            var department = new Department
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();

            var result = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };

            return ResultDto<DepartmentDto>.Success(result, "Department created successfully.");
        }

        public async Task<ResultDto<DepartmentDto>> UpdateAsync(UpdateDepartmentDto dto)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(dto.Id);

            if (department is null)
            {
                return ResultDto<DepartmentDto>.Failure("Department not found.");
            }

            department.Name = dto.Name;
            department.Description = dto.Description;

            _unitOfWork.Departments.Update(department);
            await _unitOfWork.SaveChangesAsync();

            var result = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };

            return ResultDto<DepartmentDto>.Success(result, "Department updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);

            if (department is null)
            {
                return ResultDto<bool>.Failure("Department not found.");
            }

            var hasEmployees = _unitOfWork.Employees
                .GetQueryable()
                .Any(employee => employee.DepartmentId == id);

            if (hasEmployees)
            {
                return ResultDto<bool>.Failure("This department has employees. Delete or move employees first.");
            }

            _unitOfWork.Departments.Delete(department);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Department deleted successfully.");
        }
    }
}
