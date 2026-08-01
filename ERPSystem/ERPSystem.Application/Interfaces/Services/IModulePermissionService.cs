using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.ModulePermissions;
using ERPSystem.Domain.Common;
using ERPSystem.Domain.Enums;

namespace ERPSystem.Application.Interfaces.Services
{
    public interface IModulePermissionService
    {
        Task<ResultDto<List<ModulePermissionDto>>> GetUserModulePermissionsAsync(string userId, string moduleName);

        Task<ResultDto<bool>> UpdateUserModulePermissionsAsync(UpdateModulePermissionsDto dto);

        Task<ResultDto<UserPermissionResultDto>> CheckPermissionAsync(
            string userId,
            string moduleName,
            string actionName,
            BaseEntity? entity = null);

        Task<bool> HasPermissionAsync(
            string userId,
            string moduleName,
            string actionName,
            BaseEntity? entity = null);
    }
}