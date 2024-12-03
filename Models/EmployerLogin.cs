using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Numerics;

namespace CompanyPortal.Models
{
    public class EmployerLogin
    {
        public int ID { get;  set; }

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

        [NotMapped]
        public IFormFile UploadedFile { get; set; }

        // Path to the uploaded image for preview
        public string FilePath { get; set; }

        public EmployerLogin() { }

        public EmployerLogin(IDataReader reader)
        {
            try
            {
                ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : default(Int32);
                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : default(string);
                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : default(string);
                Contact = reader["Contact"] != DBNull.Value ? reader["Contact"].ToString() : default(string);
                Department = reader["Department"] != DBNull.Value ? reader["Department"].ToString() : default(string);
            }
            catch (Exception ex)
            {
                // Log the exception if needed for debugging
                Console.WriteLine("Error mapping data: " + ex.Message);
            }
        }

    }
}