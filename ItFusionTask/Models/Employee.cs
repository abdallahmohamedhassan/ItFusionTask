using System.ComponentModel.DataAnnotations;

namespace ItFusionTask.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be under 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Join Date is required")]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid Egyptian phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } // "Male" or "Female"

        [Required(ErrorMessage = "Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a non-negative number")]
        public decimal Salary { get; set; }
    }
}
