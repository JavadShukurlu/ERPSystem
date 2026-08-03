using ERPSystem.Application.DTOs.ModulePermissions;
using ERPSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERPSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ModulePermissionsController : ControllerBase
    {
        private readonly IModulePermissionService _modulePermissionService;

        public ModulePermissionsController(IModulePermissionService modulePermissionService)
        {
            _modulePermissionService = modulePermissionService;
        }

        [HttpGet("user/{userId}/module/{moduleName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserModulePermissions(string userId, string moduleName)
        {
            var result = await _modulePermissionService.GetUserModulePermissionsAsync(userId, moduleName);

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserModulePermissions([FromBody] UpdateModulePermissionsDto dto)
        {
            if (dto is null)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Permission request body is empty.",
                    data = false
                });
            }

            if (string.IsNullOrWhiteSpace(dto.UserId))
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "UserId is required.",
                    data = false
                });
            }

            if (string.IsNullOrWhiteSpace(dto.ModuleName))
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "ModuleName is required.",
                    data = false
                });
            }

            if (dto.Permissions is null || !dto.Permissions.Any())
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "At least one permission is required.",
                    data = false
                });
            }

            var result = await _modulePermissionService.UpdateUserModulePermissionsAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("my/module/{moduleName}")]
        public async Task<IActionResult> GetMyModulePermissions(string moduleName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var result = await _modulePermissionService.GetUserModulePermissionsAsync(userId, moduleName);

            return Ok(result);
        }
    }
}