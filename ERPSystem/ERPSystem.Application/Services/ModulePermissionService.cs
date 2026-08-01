using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.ModulePermissions;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Common;
using ERPSystem.Domain.Entities;
using ERPSystem.Domain.Enums;

namespace ERPSystem.Application.Services
{
    public class ModulePermissionService : IModulePermissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ModulePermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<List<ModulePermissionDto>>> GetUserModulePermissionsAsync(string userId, string moduleName)
        {
            var permissions = await _unitOfWork.ModulePermissions.GetAllAsync();

            var result = permissions
                .Where(permission =>
                    permission.UserId == userId &&
                    permission.ModuleName == moduleName)
                .Select(permission => new ModulePermissionDto
                {
                    Id = permission.Id,
                    UserId = permission.UserId,
                    UserFullName = permission.User?.FullName,
                    ModuleName = permission.ModuleName,
                    ActionName = permission.ActionName,
                    AccessLevel = permission.AccessLevel
                })
                .ToList();

            return ResultDto<List<ModulePermissionDto>>.Success(result);
        }

        public async Task<ResultDto<bool>> UpdateUserModulePermissionsAsync(UpdateModulePermissionsDto dto)
        {
            var allPermissions = await _unitOfWork.ModulePermissions.GetAllAsync();

            var userModulePermissions = allPermissions
                .Where(permission =>
                    permission.UserId == dto.UserId &&
                    permission.ModuleName == dto.ModuleName)
                .ToList();

            foreach (var permissionDto in dto.Permissions)
            {
                var permission = userModulePermissions.FirstOrDefault(existingPermission =>
                    existingPermission.ActionName == permissionDto.ActionName);

                if (permission is null)
                {
                    permission = new ModulePermission
                    {
                        UserId = dto.UserId,
                        ModuleName = dto.ModuleName,
                        ActionName = permissionDto.ActionName,
                        AccessLevel = permissionDto.AccessLevel
                    };

                    await _unitOfWork.ModulePermissions.AddAsync(permission);
                }
                else
                {
                    permission.AccessLevel = permissionDto.AccessLevel;
                    _unitOfWork.ModulePermissions.Update(permission);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Permissions updated successfully.");
        }

        public async Task<ResultDto<UserPermissionResultDto>> CheckPermissionAsync(
            string userId,
            string moduleName,
            string actionName,
            BaseEntity? entity = null)
        {
            var hasPermission = await HasPermissionAsync(userId, moduleName, actionName, entity);

            var permissions = await _unitOfWork.ModulePermissions.GetAllAsync();

            var permission = permissions.FirstOrDefault(item =>
                item.UserId == userId &&
                item.ModuleName == moduleName &&
                item.ActionName == actionName);

            var accessLevel = permission?.AccessLevel ?? PermissionAccessLevel.None;

            var result = new UserPermissionResultDto
            {
                ModuleName = moduleName,
                ActionName = actionName,
                AccessLevel = accessLevel,
                CanAccess = hasPermission
            };

            return ResultDto<UserPermissionResultDto>.Success(result);
        }

        public async Task<bool> HasPermissionAsync(
            string userId,
            string moduleName,
            string actionName,
            BaseEntity? entity = null)
        {
            var permissions = await _unitOfWork.ModulePermissions.GetAllAsync();

            var permission = permissions.FirstOrDefault(item =>
                item.UserId == userId &&
                item.ModuleName == moduleName &&
                item.ActionName == actionName);

            if (permission is null || permission.AccessLevel == PermissionAccessLevel.None)
            {
                return false;
            }

            if (permission.AccessLevel == PermissionAccessLevel.All)
            {
                return true;
            }

            if (permission.AccessLevel == PermissionAccessLevel.Own)
            {
                if (entity is null)
                {
                    return true;
                }

                return entity.CreatedByUserId == userId;
            }

            return false;
        }
    }
}