using CompanyPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;



namespace CompanyPortal.Controllers
{
    public class EmployerController : Controller
    {

        SqlConnection con = new SqlConnection("Server=(localdb)\\begum;Database=CompanyPortal;Trusted_Connection=True;TrustServerCertificate=True");

        private readonly IConfiguration _configuration;   //from here till line 50 is about calling 
                                                          //dropdown list from the database
        public EmployerController(IConfiguration configuration)
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





        public IActionResult EmployerForm(int id)
        {
            // Initialize the model
            var model = new EmployerLogin();

            // Fetch departments and populate the dropdown options
            List<DepartmentSelection> departments = GetDepartments();

            model.Departments = departments.Select(d => new SelectListItem
            {
                Value = d.DepartmentID.ToString(),  // Use DepartmentID as the dropdown value
                Text = d.Department                // Use Department name as the display text
            }).ToList();

            if (id != default) // Check if an ID is provided for editing
            {
                using (SqlConnection con = new SqlConnection("Server=(localdb)\\begum;Database=CompanyPortal;Trusted_Connection=True;TrustServerCertificate=True"))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("get_Employer", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", id);

                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) // Populate the model if data exists
                            {
                               
                                model.Name = reader.GetString(reader.GetOrdinal("Name"));
                                model.Email = reader.GetString(reader.GetOrdinal("Email"));
                                model.Contact = reader.GetString(reader.GetOrdinal("Contact"));
                                model.Department = reader.GetString(reader.GetOrdinal("Department"));
                            }
                        }
                    }
                }
            }

            // Set the view title
            ViewBag.Title = id != default ? "Edit Employer" : "Add Employer";

            // Return the view with the model
            return View(model);
        }





        [HttpPost]
        public IActionResult EmployerForm(EmployerLogin obj)
        {
            ModelState.Remove("DepartmentList");
            ModelState.Remove("Departments");
            ModelState.Remove("filePath");
            ModelState.Remove("UploadedFile");

            if (obj.ID == default(int))
            {

                if (obj.Contact.Length < 10)
                {
                    ModelState.AddModelError("Contact", "Contact number must be at least 10 digits.");
                    return View("_ErrorView", new ErrorViewModel { ContactError = "Contact number must be at least 10 digits." });
                }

                static bool IsContactDuplicate(string contact)
                {

                    using (SqlConnection connection = new SqlConnection("Server=(localdb)\\begum;Database=CompanyPortal;Trusted_Connection=True;TrustServerCertificate=True"))
                    {

                        string query = "SELECT COUNT(1) FROM Employer WHERE Contact = @Contact";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Contact", contact);

                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        connection.Close();

                        return count > 0; // Return true if a duplicate is found
                    }
                }
                if (IsContactDuplicate(obj.Contact))
                {
                    // Add a model validation error if the contact already exists
                    ModelState.AddModelError("Contact", "This contact number is already in use.");
                    return View("_ErrorView", new ErrorViewModel { ContactError = "This contact number is already in use." });
                }
            }

            if (ModelState.IsValid)
            {
                if (obj.UploadedFile != null) //used to upload image in forms and also manage preview
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + obj.UploadedFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.UploadedFile.CopyTo(fileStream);
                    }

                    // Save the relative file path for preview
                    obj.FilePath = "/uploads/" + uniqueFileName;
                }


                if (obj.UploadedFile != null)  //this method stores the image which you are uploading in your local system 
                {
                    // Define the local folder path where the image will be saved
                    string localFolder = @"C:\Users\Atul\Desktop\mvc_images"; // Change this to your desired folder
                    Directory.CreateDirectory(localFolder); // Ensure the directory exists

                    // Create a unique filename based on the current timestamp and the original file extension
                    string fileExtension = Path.GetExtension(obj.UploadedFile.FileName);
                    string uniqueFileName = $"Employer_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";

                    string localFilePath = Path.Combine(localFolder, uniqueFileName);

                    using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                    {
                        obj.UploadedFile.CopyTo(fileStream);
                    }

                    // Save the local file path for reference (optional)
                    obj.FilePath = localFilePath; // Save the full local path if needed
                }


                using (var con = new SqlConnection("Server=(localdb)\\begum;Database=CompanyPortal;Trusted_Connection=True;TrustServerCertificate=True"))
                {
                    con.Open();
                    SqlCommand cmd;
                    if (obj.ID == 0) {
                        cmd = new SqlCommand("AddEmployer", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        // to make the procedure hit,this cmnd is used                     
                    }
                    else
                    {
                        cmd = new SqlCommand("Update_emp", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        // to make the procedure hit,this cmnd is used
                        cmd.Parameters.AddWithValue("@Id", obj.ID);                      
                    }
                    cmd.Parameters.AddWithValue("@Name", obj.Name);
                    cmd.Parameters.AddWithValue("@Email", obj.Email);
                    cmd.Parameters.AddWithValue("@Contact", obj.Contact);
                    cmd.Parameters.AddWithValue("@Department", obj.Department);
                    var res = cmd.ExecuteNonQuery();
                    if (res > 0) {

                        TempData["msg"] = obj.ID == 0 ? "New Employer Added" : "Employee details submitted succesfully";
                    }
                    else
                    {
                        TempData["msg"] = "Failed to Submit Data";
                    }
                }
                con.Close();
                ModelState.Clear();
                return RedirectToAction("EmployerList", "Employer", new { area = "" });
            }
        
                return View("EmployerForm",obj);
          }
        

        public IActionResult EmployerList(string sortOrder)
        {
            // ViewBag message to pass to the view
            ViewData["info"] = "Here is the list of employers";

            List<EmployerLogin> employerList = new List<EmployerLogin>();

            try
            {
                // Open the connection
                con.Open();


                // Use SqlCommand to execute the stored procedure
                SqlCommand cmd = new SqlCommand("SelectAllEmployer", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Execute the reader and fetch data
                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    EmployerLogin emp = new EmployerLogin(reader); // EmployerLogin model has a constructor that takes an IDataReader
                    employerList.Add(emp);
                }

                if (sortOrder == "desc")
                {
                    employerList = employerList.OrderByDescending(e => e.Name).ToList();
                }
                else
                if (sortOrder == "asc")
                {
                    employerList = employerList.OrderBy(e => e.Name).ToList(); // Default is ascending
                } else
                if (sortOrder == "Olddesc")
                {
                    employerList = employerList.OrderByDescending(e => e.ID).ToList(); 
                } else
                if (sortOrder == "Newasc")
                {
                    employerList = employerList.OrderBy(e => e.ID).ToList();
                }
             
                ViewData["CurrentSortOrder"] = sortOrder;
              

            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework)
                Console.WriteLine("Error: " + ex.Message);
                // You can also return an error view if needed
                return View("_ErrorView", new ErrorViewModel { ErrorMsg = "This contact number is already in use." });  // Create an Error view to handle failures
            }
            finally
            {
                // Ensure the connection is closed in all cases
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

            // Pass the list of employees to the view
            return View(employerList);
        }



        public IActionResult Delete(int id)
        {
            if (id != default(int))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DeleteEmployer", con);
                cmd.CommandType = CommandType.StoredProcedure; //to use any procedure we write 
                cmd.Parameters.AddWithValue("@ID", id); // Pass the userID dynamically
                var res = cmd.ExecuteNonQuery();
                con.Close();

            }
            TempData["msg"] = "user is deleted";
            return RedirectToAction("EmployerList");
        }
    }
}

