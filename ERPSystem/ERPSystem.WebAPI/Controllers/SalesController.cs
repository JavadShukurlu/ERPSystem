using ERPSystem.Application.DTOs.Sales;
using ERPSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Sales,Accountant")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _saleService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Sales,Accountant")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _saleService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<IActionResult> Create(CreateSaleDto dto)
        {
            var result = await _saleService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _saleService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
