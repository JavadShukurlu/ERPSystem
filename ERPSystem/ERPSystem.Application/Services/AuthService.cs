using ERPSystem.Application.Common;
using ERPSystem.Application.DTOs.Auth;
using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<ResultDto<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser is not null)
            {
                return ResultDto<AuthResponseDto>.Failure("Email already exists.");
            }

            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                return ResultDto<AuthResponseDto>.Failure(errors);
            }

            await EnsureRolesCreatedAsync();

            await _userManager.AddToRoleAsync(user, "Admin");

            var response = await GenerateAuthResponseAsync(user);

            return ResultDto<AuthResponseDto>.Success(response, "User registered successfully.");
        }

        public async Task<ResultDto<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserNameOrEmail)
                ?? await _userManager.FindByEmailAsync(dto.UserNameOrEmail);

            if (user is null)
            {
                return ResultDto<AuthResponseDto>.Failure("Invalid username/email or password.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
            {
                return ResultDto<AuthResponseDto>.Failure("Invalid username/email or password.");
            }

            var response = await GenerateAuthResponseAsync(user);

            return ResultDto<AuthResponseDto>.Success(response, "Login successful.");
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!)
        };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles
            };
        }

        private async Task EnsureRolesCreatedAsync()
        {
            string[] roles = { "Admin", "Manager", "Accountant", "Warehouse", "Sales" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new AppRole { Name = role });
                }
            }
        }
    }
}
