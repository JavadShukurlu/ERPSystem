using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Categories;
using ERPSystem.WebMVC.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductsController : Controller
{
    private readonly ApiService _apiService;

    public ProductsController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        var response = await _apiService.GetAsync<ApiResponse<List<ProductViewModel>>>("api/Products");

        return View(response?.Data ?? new List<ProductViewModel>());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        var model = new CreateProductViewModel
        {
            Categories = await GetCategorySelectListAsync()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategorySelectListAsync();
            return View(model);
        }

        var response = await _apiService.PostAsync<CreateProductViewModel, ApiResponse<ProductViewModel>>(
            "api/Products",
            model);

        if (response is null || !response.IsSuccess)
        {
            ViewBag.Error = response?.Message ?? "Product could not be created.";
            model.Categories = await GetCategorySelectListAsync();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        var response = await _apiService.GetAsync<ApiResponse<ProductViewModel>>($"api/Products/{id}");

        if (response?.Data is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = new UpdateProductViewModel
        {
            Id = response.Data.Id,
            Name = response.Data.Name,
            SKU = response.Data.SKU,
            Description = response.Data.Description,
            PurchasePrice = response.Data.PurchasePrice,
            SalePrice = response.Data.SalePrice,
            CategoryId = response.Data.CategoryId,
            Categories = await GetCategorySelectListAsync()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateProductViewModel model)
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        if (id != model.Id)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategorySelectListAsync();
            return View(model);
        }

        var response = await _apiService.PutAsync<UpdateProductViewModel, ApiResponse<ProductViewModel>>(
            $"api/Products/{id}",
            model);

        if (response is null || !response.IsSuccess)
        {
            ViewBag.Error = response?.Message ?? "Product could not be updated.";
            model.Categories = await GetCategorySelectListAsync();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Products/{id}");

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetCategorySelectListAsync()
    {
        var response = await _apiService.GetAsync<ApiResponse<List<CategoryViewModel>>>("api/Categories");

        return response?.Data?
            .Select(category => new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = category.Name
            })
            .ToList() ?? new List<SelectListItem>();
    }

    private bool IsAdminLoggedIn()
    {
        var token = HttpContext.Session.GetString("JWToken");
        var isAdmin = HttpContext.Session.GetString("IsAdmin");

        return !string.IsNullOrWhiteSpace(token) && isAdmin == "true";
    }

    private IActionResult RedirectToAdminLogin()
    {
        return RedirectToAction("Login", "Auth", new { area = "Admin" });
    }
}