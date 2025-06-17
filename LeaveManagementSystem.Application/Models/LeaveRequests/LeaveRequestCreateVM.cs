using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Application.Models.LeaveRequests
{
    public class LeaveRequestCreateVM : IValidatableObject
    {
        [DisplayName("State Date")]
        [Required]
        public DateOnly StartDate { get; set; }

        [DisplayName("End Date")]
        [Required]
        public DateOnly EndDate { get; set; }

        [DisplayName("Desired Leave Type")]
        [Required]
        public int LeaveTypeId { get; set; }

        [DisplayName("Additional Information")]
        public string? RequestComments { get; set; }

        public SelectList? LeaveTypes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                // Note: The second params lists the key names that this validation result applies to
                // old syntax yield return new ValidationResult("The start date cannot be after the end date.", new[] { nameof(StartDate), nameof(EndDate) });
                yield return new ValidationResult("The start date cannot be after the end date.", [nameof(StartDate), nameof(EndDate)]);
            }
        }
    }
}