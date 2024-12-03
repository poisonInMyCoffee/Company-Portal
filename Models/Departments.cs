using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CompanyPortal.Models
{
    public class DepartmentSelection
    {
        public int DepartmentID { get; set; }
        [Required(ErrorMessage="Department is required")]
        public string Department { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }
    }

}
