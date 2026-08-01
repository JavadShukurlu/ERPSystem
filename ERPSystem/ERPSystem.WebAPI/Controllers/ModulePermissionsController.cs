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
        public async Task<IActionResult> UpdateUserModulePermissions(UpdateModulePermissionsDto dto)
        {
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