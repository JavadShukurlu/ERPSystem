using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoriesController : Controller
{
    private readonly ApiService _apiService;

    public CategoriesController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        var response = await _apiService.GetAsync<ApiResponse<List<CategoryViewModel>>>("api/Categories");

        return View(response?.Data ?? new List<CategoryViewModel>());
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
    public async Task<IActionResult> Create(CreateCategoryViewModel model)
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAdminLogin();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await _apiService.PostAsync<CreateCategoryViewModel, ApiResponse<CategoryViewModel>>(
            "api/Categories",
            model);

        if (response is null || !response.IsSuccess)
        {
            ViewBag.Error = response?.Message ?? "Category could not be created.";
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

        var response = await _apiService.GetAsync<ApiResponse<CategoryViewModel>>($"api/Categories/{id}");

        if (response?.Data is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = new UpdateCategoryViewModel
        {
            Id = response.Data.Id,
            Name = response.Data.Name,
            Description = response.Data.Description
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateCategoryViewModel model)
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

        var response = await _apiService.PutAsync<UpdateCategoryViewModel, ApiResponse<CategoryViewModel>>(
            $"api/Categories/{id}",
            model);

        if (response is null || !response.IsSuccess)
        {
            ViewBag.Error = response?.Message ?? "Category could not be updated.";
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

        await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Categories/{id}");

        return RedirectToAction(nameof(Index));
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