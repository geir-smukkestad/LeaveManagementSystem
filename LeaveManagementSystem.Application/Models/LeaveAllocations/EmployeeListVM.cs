using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Application.Models.LeaveAllocations
{
    public class EmployeeListVM
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;

    }
}
