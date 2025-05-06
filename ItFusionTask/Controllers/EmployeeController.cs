using ItFusionTask.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace ItFusionTask.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly HttpClient _httpClient;

        public EmployeeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EmployeeApi");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/EmployeeApi");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var employees = JsonSerializer.Deserialize<List<Employee>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(employees);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<Employee>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            try
            {
                var json = JsonSerializer.Serialize(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/EmployeeApi", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Employee Created successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                TempData["Error"] = "An error occurred while Employee Created.";

                return View(employee);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/EmployeeApi/{id}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var employee = JsonSerializer.Deserialize<Employee>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(employee);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            try
            {
                var json = JsonSerializer.Serialize(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/EmployeeApi/{employee.Id}", content);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Employee Updated successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                TempData["Error"] = "An error occurred while Employee Updated.";

                return View(employee);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/EmployeeApi/{id}");
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Employee Deleted successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                TempData["Error"] = "An error occurred while Employee Deleted.";

                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError("", "Please select a file.");
                    return View();
                }


                using var content = new MultipartFormDataContent
        {
            { new StreamContent(file.OpenReadStream()), "file", file.FileName }
        };

                var response = await _httpClient.PostAsync("api/EmployeeApi/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Employees uploaded successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Warning"] = $"Upload failed: {error}";

                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while uploading.";

                return View();
            }
        }

    }
}
