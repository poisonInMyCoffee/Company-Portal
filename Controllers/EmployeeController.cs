using CompanyPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;

namespace CompanyPortal.Controllers
{
    public class EmployeeController : Controller
    {
        SqlConnection con = new SqlConnection("Server=(localdb)\\begum;Database=CompanyPortal;Trusted_Connection=True;TrustServerCertificate=True");

        private readonly IConfiguration _configuration;   //from here till line 50 is about calling 
                                                          //dropdown list from the database
        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private List<DepartmentSelection> GetDepartments()
        {
            List<DepartmentSelection> departmentList = new List<DepartmentSelection>();

            con.Open();
            string query = "Select id, Department FROM Departments";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DepartmentSelection department = new DepartmentSelection
                        {
                            DepartmentID = reader.GetInt32(0),    // Get the DepartmentID
                            Department = reader.GetString(1)      // Get the Department name
                        };
                        departmentList.Add(department);
                    }
                }
            }
            con.Close();

            return departmentList;
        }
        public IActionResult DepartmentSelection()
        {
            ViewData["Message"] = "Please select your department";
            var model = new EmployeeLogin();

            // Fetch departments and populate the dropdown options
            List<DepartmentSelection> departments = GetDepartments();

            model.Departments = departments.Select(d => new SelectListItem
            {
                Value = d.DepartmentID.ToString(),  // Use DepartmentID as the dropdown value
                Text = d.Department               // Use Department name as the display text
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult DepartmentSelection(int departmentId)
        {
            // Fetch list of employees based on the selected department
            List<EmployeeLogin> employees = GetEmployeesByDepartment(departmentId);

            // Return a view that displays the list of employees in the selected department
            return View("EmployerList", employees);  // assuming you have an EmployeeList view to show employees
        }

        // Method to fetch employees for a given department using the stored procedure "Get_List"
        private List<EmployeeLogin> GetEmployeesByDepartment(int departmentId)
        {
            List<EmployeeLogin> employeeList = new List<EmployeeLogin>();

            // Open connection and execute stored procedure
            con.Open();
            using (SqlCommand cmd = new SqlCommand("Get_List", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentID", departmentId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EmployeeLogin employee = new EmployeeLogin
                        {
                            ID = reader.GetInt32(0),   // Employee ID from the result
                            Name = reader.GetString(1) // Employee name from the result
                        };
                        employeeList.Add(employee);
                    }
                }
            }
            con.Close();

            return employeeList;
        }
    

        public IActionResult Index()
        {
            ViewData["Message"] = "Select appropriate response";
            return View();
        }public IActionResult EmployeeForm()
        {
            
            return View();
        }
       
    }
}
