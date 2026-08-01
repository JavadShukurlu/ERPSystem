using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers;

[Area("Admin")]
public class SuppliersController : Controller
{
    private readonly ApiService _apiService;

    public SuppliersController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        var response = await _apiService.GetAsync<ApiResponse<List<SupplierViewModel>>>("api/Suppliers");

        return View(response?.Data ?? new List<SupplierViewModel>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSupplierViewModel model)
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var logoUrl = await SaveImageAsync(model.LogoFile, "suppliers");

        if (!string.IsNullOrWhiteSpace(logoUrl))
        {
            model.LogoUrl = logoUrl;
        }

        var response = await _apiService.PostAsync<CreateSupplierViewModel, ApiResponse<SupplierViewModel>>(
            "api/Suppliers",
            model);

        if (response is null || !response.IsSuccess)
        {
            ViewBag.Error = response?.Message ?? "Supplier could not be created.";
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

        var response = await _apiService.GetAsync<ApiResponse<SupplierViewModel>>($"api/Suppliers/{id}");

        if (response?.Data is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = new UpdateSupplierViewModel
        {
            Id = response.Data.Id,
            CompanyName = response.Data.CompanyName,
            ContactName = response.Data.ContactName,
            Email = response.Data.Email,
            PhoneNumber = response.Data.PhoneNumber,
            Address = response.Data.Address,
            LogoUrl = response.Data.LogoUrl
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateSupplierViewModel model)
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
            return View(model);
        }

        var logoUrl = await SaveImageAsync(model.LogoFile, "suppliers");

        if (!string.IsNullOrWhiteSpace(logoUrl))
        {
            model.LogoUrl = logoUrl;
        }

        var response = await _apiService.PutAsync<UpdateSupplierViewModel, ApiResponse<SupplierViewModel>>(
            $"api/Suppliers/{id}",
            model);

        if (response is null || !response.IsSuccess)
        {
            ViewBag.Error = response?.Message ?? "Supplier could not be updated.";
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

        await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Suppliers/{id}");

        return RedirectToAction(nameof(Index));
    }

    private async Task<string?> SaveImageAsync(IFormFile? file, string folderName)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Only jpg, jpeg, png and webp files are allowed.");
        }

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads",
            folderName);

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