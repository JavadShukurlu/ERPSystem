using ERPSystem.Application.DTOs.Invoices;
using ERPSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Accountant,Sales")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _invoiceService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Accountant,Sales")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _invoiceService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Accountant,Sales")]
        public async Task<IActionResult> Create(CreateInvoiceDto dto)
        {
            var result = await _invoiceService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        [HttpPatch("status")]
        [Authorize(Roles = "Admin,Manager,Accountant")]
        public async Task<IActionResult> UpdateStatus(UpdateInvoiceStatusDto dto)
        {
            var result = await _invoiceService.UpdateStatusAsync(dto);

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
            var result = await _invoiceService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
