using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Categories;
using ERPSystem.WebMVC.ViewModels.Products;
using ERPSystem.WebMVC.ViewModels.Stocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPSystem.WebMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApiService apiService, IWebHostEnvironment webHostEnvironment)
        {
            _apiService = apiService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var productsResponse = await _apiService.GetAsync<ApiResponse<List<ProductViewModel>>>("api/Products");
            var stocksResponse = await _apiService.GetAsync<ApiResponse<List<StockViewModel>>>("api/Stocks");

            var products = productsResponse?.Data ?? new List<ProductViewModel>();
            var stocks = stocksResponse?.Data ?? new List<StockViewModel>();

            foreach (var product in products)
            {
                product.StockQuantity = stocks
                    .Where(stock => stock.ProductId == product.Id)
                    .Sum(stock => stock.Quantity);
            }

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new CreateProductViewModel();

            await LoadCategoriesAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile, "products");

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }

            var response = await _apiService.PostAsync<CreateProductViewModel, ApiResponse<ProductViewModel>>(
                "api/Products",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Product could not be created.";
                await LoadCategoriesAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _apiService.GetAsync<ApiResponse<ProductViewModel>>($"api/Products/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateProductViewModel
            {
                Id = response.Data.Id,
                Name = response.Data.Name,
                SKU = response.Data.SKU,
                Description = response.Data.Description,
                ImageUrl = response.Data.ImageUrl,
                PurchasePrice = response.Data.PurchasePrice,
                SalePrice = response.Data.SalePrice,
                CategoryId = response.Data.CategoryId
            };

            await LoadCategoriesAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProductViewModel model)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile, "products");

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }

            var response = await _apiService.PutAsync<UpdateProductViewModel, ApiResponse<ProductViewModel>>(
                $"api/Products/{model.Id}",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Product could not be updated.";
                await LoadCategoriesAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Products/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategoriesAsync(CreateProductViewModel model)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<CategoryViewModel>>>("api/Categories");

            model.Categories = response?.Data?
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                })
                .ToList() ?? new List<SelectListItem>();
        }

        private async Task LoadCategoriesAsync(UpdateProductViewModel model)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<CategoryViewModel>>>("api/Categories");

            model.Categories = response?.Data?
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                })
                .ToList() ?? new List<SelectListItem>();
        }

        private async Task<string?> SaveImageAsync(IFormFile? file, string folderName)
        {
            if (file is null || file.Length == 0)
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folderName}/{fileName}";
        }

        private bool IsUserLoggedIn()
        {
            var token = HttpContext.Session.GetString("JWToken");

            return !string.IsNullOrWhiteSpace(token);
        }
    }
}