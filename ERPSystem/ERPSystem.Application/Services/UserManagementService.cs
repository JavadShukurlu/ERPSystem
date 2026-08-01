using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Users;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserManagementService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResultDto<List<UserDto>>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return ResultDto<List<UserDto>>.Success(result);
        }

        public async Task<ResultDto<UserDto>> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return ResultDto<UserDto>.Failure("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var result = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };

            return ResultDto<UserDto>.Success(result);
        }

        public async Task<ResultDto<UserDto>> CreateAsync(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.UserName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return ResultDto<UserDto>.Failure("Full name, username, email and password are required.");
            }

            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
            {
                var errorMessage = string.Join(", ", createResult.Errors.Select(error => error.Description));
                return ResultDto<UserDto>.Failure(errorMessage);
            }

            if (dto.Roles.Any())
            {
                foreach (var roleName in dto.Roles)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(roleName);

                    if (!roleExists)
                    {
                        return ResultDto<UserDto>.Failure($"Role '{roleName}' does not exist.");
                    }
                }

                var roleResult = await _userManager.AddToRolesAsync(user, dto.Roles);

                if (!roleResult.Succeeded)
                {
                    var errorMessage = string.Join(", ", roleResult.Errors.Select(error => error.Description));
                    return ResultDto<UserDto>.Failure(errorMessage);
                }
            }

            var roles = await _userManager.GetRolesAsync(user);

            var result = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };

            return ResultDto<UserDto>.Success(result, "User created successfully.");
        }

        public async Task<ResultDto<UserDto>> AssignRolesAsync(AssignUserRolesDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user is null)
            {
                return ResultDto<UserDto>.Failure("User not found.");
            }

            foreach (var roleName in dto.Roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    return ResultDto<UserDto>.Failure($"Role '{roleName}' does not exist.");
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                var errorMessage = string.Join(", ", removeResult.Errors.Select(error => error.Description));
                return ResultDto<UserDto>.Failure(errorMessage);
            }

            if (dto.Roles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, dto.Roles);

                if (!addResult.Succeeded)
                {
                    var errorMessage = string.Join(", ", addResult.Errors.Select(error => error.Description));
                    return ResultDto<UserDto>.Failure(errorMessage);
                }
            }

            var updatedRoles = await _userManager.GetRolesAsync(user);

            var result = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = updatedRoles.ToList()
            };

            return ResultDto<UserDto>.Success(result, "User roles updated successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return ResultDto<bool>.Failure("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(error => error.Description));
                return ResultDto<bool>.Failure(errorMessage);
            }

            return ResultDto<bool>.Success(true, "User deleted successfully.");
        }
    }
}