using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Employees
{
    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email format is not valid.")]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Position { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary cannot be negative.")]
        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; } = DateTime.Now;

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        public List<SelectListItem> Departments { get; set; } = new();
    }
}