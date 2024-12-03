using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CompanyPortal.Models
{
    public class EmployeeLogin
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid contact number. Please enter a 10-digit number.")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public string Department { get; set; } // Holds the selected department value

        public IEnumerable<SelectListItem> Departments { get; set; } // List of departments for dropdown

        public EmployeeLogin() { }



        public EmployeeLogin(IDataReader reader)
        {
            ID = DBNull.Value != reader["ID"] ? (Int32)reader["ID"] : default(Int32);
            Name = DBNull.Value != reader["Name"] ? (string)reader["Name"] : default(string);
            Email = DBNull.Value != reader["Email"] ? (string)reader["Email"] : default(string);
            Contact = DBNull.Value != reader["Contact"] ? (string)reader["Contact"] : default(string);
            Department = DBNull.Value != reader["Department"] ? (string)reader["Department"] : default(string);
        }
    }
}
