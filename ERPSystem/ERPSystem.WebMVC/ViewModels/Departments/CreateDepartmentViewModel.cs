using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Departments
{
    public class CreateDepartmentViewModel
    {
        [Required(ErrorMessage = "Department name is required.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
