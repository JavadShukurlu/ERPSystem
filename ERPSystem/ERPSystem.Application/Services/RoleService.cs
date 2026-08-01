using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Roles;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<ResultDto<List<RoleDto>>> GetAllAsync()
        {
            var roles = await _roleManager.Roles
                .Select(role => new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name!
                })
                .ToListAsync();

            return ResultDto<List<RoleDto>>.Success(roles);
        }

        public async Task<ResultDto<RoleDto>> CreateAsync(CreateRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ResultDto<RoleDto>.Failure("Role name is required.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(dto.Name);

            if (roleExists)
            {
                return ResultDto<RoleDto>.Failure("Role already exists.");
            }

            var role = new AppRole
            {
                Name = dto.Name
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(error => error.Description));
                return ResultDto<RoleDto>.Failure(errorMessage);
            }

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name!
            };

            return ResultDto<RoleDto>.Success(roleDto, "Role created successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role is null)
            {
                return ResultDto<bool>.Failure("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(error => error.Description));
                return ResultDto<bool>.Failure(errorMessage);
            }

            return ResultDto<bool>.Success(true, "Role deleted successfully.");
        }
    }
}