using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Categories
{
    public class UpdateCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
