using ERPSystem.Application.DTOs.Stocks;
using ERPSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Warehouse")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _stockService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Warehouse")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _stockService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("low-stock")]
        [Authorize(Roles = "Admin,Manager,Warehouse")]
        public async Task<IActionResult> GetLowStock()
        {
            var result = await _stockService.GetLowStockAsync();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Warehouse")]
        public async Task<IActionResult> Create(CreateStockDto dto)
        {
            var result = await _stockService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Warehouse")]
        public async Task<IActionResult> Update(int id, UpdateStockDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Route id and body id do not match.");
            }

            var result = await _stockService.UpdateAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("increase")]
        [Authorize(Roles = "Admin,Manager,Warehouse")]
        public async Task<IActionResult> Increase(AdjustStockDto dto)
        {
            var result = await _stockService.IncreaseAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("decrease")]
        [Authorize(Roles = "Admin,Manager,Warehouse,Sales")]
        public async Task<IActionResult> Decrease(AdjustStockDto dto)
        {
            var result = await _stockService.DecreaseAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _stockService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
