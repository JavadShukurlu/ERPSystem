using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Departments
{
    public class UpdateDepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
