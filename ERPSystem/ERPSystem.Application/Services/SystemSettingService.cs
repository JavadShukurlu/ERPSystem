using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.SystemSettings;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;

namespace ERPSystem.Application.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemSettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<SystemSettingDto>> GetAsync()
        {
            var settings = await _unitOfWork.SystemSettings.GetAllAsync();

            var setting = settings.FirstOrDefault();

            if (setting is null)
            {
                setting = new SystemSetting
                {
                    CompanyName = "ERP System",
                    Currency = "AZN",
                    InvoicePrefix = "INV",
                    LowStockThreshold = 5,
                    TaxPercent = 0
                };

                await _unitOfWork.SystemSettings.AddAsync(setting);
                await _unitOfWork.SaveChangesAsync();
            }

            var result = MapToDto(setting);

            return ResultDto<SystemSettingDto>.Success(result);
        }

        public async Task<ResultDto<SystemSettingDto>> UpdateAsync(UpdateSystemSettingDto dto)
        {
            var settings = await _unitOfWork.SystemSettings.GetAllAsync();

            var setting = settings.FirstOrDefault();

            if (setting is null)
            {
                setting = new SystemSetting();

                await _unitOfWork.SystemSettings.AddAsync(setting);
            }

            setting.CompanyName = dto.CompanyName;
            setting.CompanyEmail = dto.CompanyEmail;
            setting.CompanyPhone = dto.CompanyPhone;
            setting.Address = dto.Address;
            setting.Currency = dto.Currency;
            setting.TaxPercent = dto.TaxPercent;
            setting.InvoicePrefix = dto.InvoicePrefix;
            setting.LowStockThreshold = dto.LowStockThreshold;

            _unitOfWork.SystemSettings.Update(setting);
            await _unitOfWork.SaveChangesAsync();

            var result = MapToDto(setting);

            return ResultDto<SystemSettingDto>.Success(result, "System settings updated successfully.");
        }

        private static SystemSettingDto MapToDto(SystemSetting setting)
        {
            return new SystemSettingDto
            {
                Id = setting.Id,
                CompanyName = setting.CompanyName,
                CompanyEmail = setting.CompanyEmail,
                CompanyPhone = setting.CompanyPhone,
                Address = setting.Address,
                Currency = setting.Currency,
                TaxPercent = setting.TaxPercent,
                InvoicePrefix = setting.InvoicePrefix,
                LowStockThreshold = setting.LowStockThreshold
            };
        }
    }
}