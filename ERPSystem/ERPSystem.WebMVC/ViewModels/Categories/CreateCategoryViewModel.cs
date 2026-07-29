using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Categories
{
    public class CreateCategoryViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
