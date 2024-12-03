using CompanyPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace CompanyPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult CheckContactUnique()
        {
            return View();
        }
        public IActionResult Search_toggle()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Search_toggle(String emp_Name)
        {
            List<EmployerLogin> searchList = new List<EmployerLogin>();

            if (string.IsNullOrEmpty(emp_Name))
            {
                // If no search term is entered, you can set a message to show in the view.
                ViewBag.Message = "Please enter something to search.";
                ViewData["SearchResults"] = searchList; // Pass an empty list
                return View(searchList);
            }

            using (SqlConnection connection = new SqlConnection("Server=(localdb)\\begum;Database=CompanyPortal;Trusted_Connection=True;TrustServerCertificate=True"))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("get_empByName", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Add parameter before executing the command
                cmd.Parameters.AddWithValue("@emp_name", emp_Name + '%');  // Ensure parameter name matches the stored procedure

                // Execute the reader and fetch data
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Map the data to EmployerLogin (ensure constructor or mapping works)
                        EmployerLogin emp = new EmployerLogin
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2),
                            Contact = reader.GetString(3),
                            Department = reader.GetString(4)
                        };
                        searchList.Add(emp);
                    }
                }
            }

            if (searchList.Count == 0)
            {
                ViewBag.Message = "No items match your search.";
            }

            ViewData["SearchResults"] = searchList; // Pass the results to the view
            return View(searchList);
        }


        public IActionResult CompanyList()
            {
                ViewData["Message"] = "Please click below to see the appropriate list";
                return View();
            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }




