using ItFusionTask.Models;
using ItFusionTaskAPI.Interface;
using Newtonsoft.Json;
using System.Text.Json;

namespace ItFusionTaskAPI.service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IWebHostEnvironment _env;

        public EmployeeService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<List<Employee>> GetAllAsync()
        {
            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, "Data", "employees.json");

                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var employees = JsonConvert.DeserializeObject<List<Employee>>(json);
                    if (employees != null && employees.Count > 0)
                    {


                        return employees;
                    }
                    else { return new List<Employee>(); }
                }

                return new List<Employee>();
            }
            catch (Exception ex)
            {
                // Log exception if using logger
                Console.WriteLine($"Error reading file: {ex.Message}");
                return new List<Employee>();
            }
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            try
            {
                var employees = await GetAllAsync();
                return employees.FirstOrDefault(e => e.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting employee by ID: {ex.Message}");
                return null;
            }
        }

        public async Task AddAsync(Employee employee)
        {
            try
            {
                var employees = await GetAllAsync();
                int nextId = employees.Any() ? employees.Max(e => e.Id) + 1 : 1;
                employee.Id = nextId;
                employees.Add(employee);
                await SaveAsync(employees);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding employee: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(Employee employee)
        {
            try
            {
                var employees = await GetAllAsync();
                var index = employees.FindIndex(e => e.Id == employee.Id);
                if (index != -1)
                {
                    employees[index] = employee;
                    await SaveAsync(employees);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var employees = await GetAllAsync();
                var toRemove = employees.FirstOrDefault(e => e.Id == id);
                if (toRemove != null)
                {
                    employees.Remove(toRemove);
                    await SaveAsync(employees);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting employee: {ex.Message}");
                throw;
            }
        }

        private async Task SaveAsync(List<Employee> employees)
        {
            try
            {

                var filePath = Path.Combine(_env.ContentRootPath, "Data", "employees.json");
            var json = JsonConvert.SerializeObject(employees, Formatting.Indented);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
                throw;
            }
        }

    }
}
