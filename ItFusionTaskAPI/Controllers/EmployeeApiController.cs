using ItFusionTask.Models;
using ItFusionTaskAPI.Interface;
using ItFusionTaskAPI.service;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace ItFusionTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")] 

    public class EmployeeApiController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeApiController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var employees = await _employeeService.GetAllAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);
                if (employee == null)
                    return NotFound();

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            try
            {
                await _employeeService.AddAsync(employee);
                return Ok(new { message = "Employee added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding employee: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
        {
            try
            {
                if (id != employee.Id)
                    return BadRequest("ID mismatch.");

                await _employeeService.UpdateAsync(employee);
                return Ok(new { message = "Employee updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating employee: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _employeeService.DeleteAsync(id);
                return Ok(new { message = "Employee deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting employee: {ex.Message}");
            }
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                List<Employee> importedEmployees = new();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using var package = new ExcelPackage(stream);
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null) return BadRequest("No worksheet found");

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Skip header
                    {
                        try
                        {
                            var employee = new Employee
                            {
                                Name = worksheet.Cells[row, 1].Text,
                                JoinDate = DateTime.Parse(worksheet.Cells[row, 2].Text),
                                Phone = worksheet.Cells[row, 3].Text,
                                Gender = worksheet.Cells[row, 4].Text,
                                Salary = decimal.Parse(worksheet.Cells[row, 5].Text)
                            };

                            importedEmployees.Add(employee);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest($"Invalid data at row {row}: {ex.Message}");
                        }
                    }
                }

                foreach (var emp in importedEmployees)
                {
                    await _employeeService.AddAsync(emp);
                }

                return Ok("File uploaded and data saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

