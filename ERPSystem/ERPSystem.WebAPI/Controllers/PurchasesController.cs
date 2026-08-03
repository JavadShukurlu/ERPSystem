using ERPSystem.Application.DTOs.Purchases;
using ERPSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _purchaseService.GetAllAsync();

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _purchaseService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePurchaseDto dto)
        {
            var result = await _purchaseService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePurchaseDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Route id and body id do not match.");
            }

            var result = await _purchaseService.UpdateAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _purchaseService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}